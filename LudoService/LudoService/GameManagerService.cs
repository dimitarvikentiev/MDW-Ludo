using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;

namespace LudoService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GameManagerService : IGame, IAccount, ILobby
    {
        private List<IGameCallback> callbackList = new List<IGameCallback>();
        public List<Pawn> pawns = new List<Pawn>();
        private List<Player> players = new List<Player>();
        private List<Game> games = new List<Game>();
        private int currentTurn = 1;
        private Game game;

        public void MakeGame()
        {
            if (players.Count == 2)
            {
                game = new Game(players.ElementAt(0), players.ElementAt(1));
            }
            else if (players.Count == 3)
            {
                game = new Game(players.ElementAt(0), players.ElementAt(1), players.ElementAt(2));
            }
            else if (players.Count == 4)
            {
                game = new Game(players.ElementAt(0), players.ElementAt(1), players.ElementAt(2), players.ElementAt(3));
            }
        }

        /// <summary>
        /// Awards the winner of the game.
        /// </summary>
        /// <param name="userName">The user who has won.</param>
        /// <param name="points">The number of points won.</param>
        /// <returns></returns>
        public bool AwardWinner(string userName, int points)
        {
            return initializeConnection.Award(userName, points);

        }
        /// <summary>
        /// Sets the turn for the players.
        /// </summary>
        public void setTurn()
        {
            if (currentTurn >= players.Count)
            {
                currentTurn = 1;
            }
            else currentTurn++;
            //Player player=null;
            //int playerFinished =0;
            //foreach(Player p in players)
            //{ if (p.Priority == currentTurn) player = p; }
            //if(player!=null)
            //{ foreach (Pawn p in player.pawns) if (p.AllFinish) playerFinished++; }
            //if (playerFinished == 4) currentTurn++;

        }
        /// <summary>
        /// Check if the current player can make a move based on priority.
        /// It also checks it the pawn is owned by the player or not.
        /// </summary>
        /// <param name="player">The current player.</param>
        /// <param name="pawn">The pawn that the player wants to move.</param>
        /// <returns></returns>
        public bool getTurn(string player, Pawn pawn)
        {
            foreach (Player p in players)
            {
                if (p.Username == player && p.Priority == currentTurn)
                {
                    foreach (Pawn pn in p.pawns)
                    {
                        if (pn.PawnColor == pawn.PawnColor && pn.PawnID == pawn.PawnID)
                            return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if the player is allowed to roll the die.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool diceTurn(string player)
        {
            foreach (Player p in players)
            {
                if (p.Username == player && p.Priority == currentTurn)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Gets a list of the players from the database together with their points.
        /// </summary>
        /// <returns></returns>
        public List<string> getRanking()
        {

            List<string> rankings = initializeConnection.getRankingDB();
            rankings.Sort();
            return rankings;
        }
        /// <summary>
        /// Resets the pawn to its initial postion.
        /// </summary>
        /// <param name="p">The pawn that needs to be respawned.</param>
        public void ResetPawn(Pawn p)
        {
            var currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();
            foreach (var cb in callbackList)
            {
                if (cb != currentCallback)
                {
                    cb.RespawNotify(p);
                }
            }
        }
        /// <summary>
        /// Adds a new player to the list.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="nbOfPoints">Current number of points</param>
        /// <param name="position">His/her position among other players</param>
        /// <param name="priority"></param>
        public void CreatePlayer(string username, int nbOfPoints, int position, int priority)
        {
            players.Add(new Player(username, nbOfPoints, position, priority));
        }

        /// <summary>
        /// Finds and removes a player from the list.
        /// </summary>
        /// <param name="username">THe username of the player.</param>
        public void RemovePlayer(string username)
        {
            Player player = null;
            bool b = false;
            foreach (Player p in players)
            {
                if (p.Username == username)
                {
                    player = p;
                    if (currentTurn == p.Priority)
                        currentTurn++;
                  
                        b = true;
                }
            }

            if (b)
            {
                players.Remove(player);
            }

            IGameCallback currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();

            foreach (var cb in callbackList)
            {
                if (cb != currentCallback)
                {
                    cb.MessageRecieved("Player " + username + " left the game.", "SYSTEM");
                }
            }
            ReasignPriority();
        }

        /// <summary>
        ///  Connects a new user.
        /// </summary>
        /// <returns>Returna a list of players.</returns>
        public List<Player> Connect(String username, int nbOfPoints, int Position, int priority)
        {
            IGameCallback currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();

            if (!callbackList.Contains(currentCallback))
            {
                callbackList.Add(currentCallback);
                Player p = (new Player(username, nbOfPoints, Position, priority));
                players.Add(p);
                p.pawns = new List<Pawn> {
                        new Pawn(1, p),
                        new Pawn(2, p),
                        new Pawn(3, p),
                        new Pawn(4, p)
                };
            }

            foreach (var v in callbackList)
            {
                if (v != currentCallback)
                {
                    v.NewPlayerConnected(players);
                }
            }
            return players;
        }

        /// <summary>
        /// Disconnects the player by removing the current callback from the list.
        /// </summary>
        public void Disconnect()
        {
            IGameCallback currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();
            callbackList.Remove(currentCallback);

            foreach (var v in callbackList)
            {
                if (v != currentCallback)
                {
                    v.NewPlayerConnected(players);
                }
            }          
        }

        /// <summary>
        /// Sends a message to other players.
        /// </summary>
        /// <param name="msg">The message to be sent.</param>
        /// <param name="playerName">The player who is sending the message.</param>
        public void SendMessage(string msg, string playerName)
        {
            IGameCallback currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();

            foreach (var cb in callbackList)
            {
                if (cb != currentCallback)
                {
                    cb.MessageRecieved(msg, playerName);
                }
            }
        }

        /// <summary>
        /// Returns information about a player.
        /// </summary>
        /// <param name="username">Based on his username.</param>
        /// <returns></returns>
        public string GetPlayerInfo(string username)
        {
            var currentCallBack = OperationContext.Current.GetCallbackChannel<IGameCallback>();
            foreach (Player p in players)
            {
                if (p.Username == username) return p.Username + " " + p.Position + " " + p.NbOfPoints;
            }
            return null;
        }
 
        /// <returns>List of all connected players.</returns>
        public List<Player> GetPlayers()
        {
            return this.players;
        }
        /// <summary>
        /// Notifies other players that a pawn has moved.
        /// </summary>
        /// <param name="pawn">The pawn to be moved.</param>
        /// <param name="newPosition">The pawn's new position.</param>
        public void MoveForward(Pawn pawn, int newPosition)
        {
            var currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();
            foreach (var cb in callbackList)
            {
                if (cb != currentCallback)
                {
                    cb.PawnNotify(pawn, newPosition);
                }
            }
        }
        /// <summary>
        /// Generates a random number between 1 and 6 
        /// and informs other players about the result.
        /// </summary>
        /// <returns>Random number between 1 and 6</returns>
        public int ThrowDice(String playerName)
        {
            Random dieResult = new Random();
            int result = dieResult.Next(1, 7);
            var currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();
            foreach (var cb in callbackList)
            {
                if (cb != currentCallback)
                {
                    cb.DiceNotify(result, playerName);
                }
            }

            return result;
        }
        /// <summary>
        /// Initializes the connection with the database.
        /// </summary>
        public DatabaseHelper initializeConnection = new DatabaseHelper();

        /// <summary>
        /// Registeres a new user.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="username">Chosen username.</param>
        /// <param name="password">Chosen password for the account.</param>
        /// <returns>True if the action was successful. Otherwise false.</returns>
        public bool createAccount(String name, String username, String password)
        {
            return initializeConnection.createAccount(name, username, password);
        }
       
        public string LogIn(String username, string password)
        {
            string newPlayer = initializeConnection.logIn(username, password);

            return newPlayer;
        }
        /// <summary>
        /// Pauses the game for all users.
        /// </summary>
        /// <param name="b"></param>
        public void GamePause(bool b)
        {
            IGameCallback currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();

            foreach (var cb in callbackList)
            {
                if (cb != currentCallback)
                {
                    cb.GamePaused(b);
                }
            }
        }
        /// <summary>
        /// Checks if all the pawns of a player have reached the final position.
        /// </summary>
        /// <param name="playerName">The player whose pawns have to be checked.</param>
        /// <returns>Yes if all the players have the maximum position. Otherwise false.</returns>
        Player pplayer;
        public bool AllFinish(string playerName)
        {
            int i = 0;
            foreach (Player p in players)
            {
                if (p.Username == playerName)
                {
                    pplayer = p;
                    foreach (Pawn pn in p.pawns)
                    {
                        if (pn.AllFinish == true)
                        {
                            i++;
                        }
                    }
                }
            }
            if (i == 4)
            {
                MakeGame();
                int n = game.SetWinner(pplayer);
                if (n > 0)
                {
                    AwardWinner(pplayer.Username, n);
                }
                return true;
            }
            else return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="playerName"></param>
        public void PawnHasFinish(Pawn pawn, string playerName)
        {
            foreach (Player p in players)
            {
                if (p.Username == playerName)
                {
                    foreach (Pawn pn in p.pawns)
                    {
                        if (pn.PawnID == pawn.PawnID)
                            pn.AllFinish = true;
                    }
                }
            }
        }
        /// <summary>
        /// Notifies other players that there is a new winner.
        /// </summary>
        /// <param name="playerName">The player who has won the game.</param>
        public void NotifyForWinner(string playerName)
        {
            IGameCallback currentCallback = OperationContext.Current.GetCallbackChannel<IGameCallback>();

            foreach (var cb in callbackList)
            {
                if (cb != currentCallback)
                {
                    cb.NewWinner(playerName);
                }
            }
        }
        public void InvitePlayers(string player2, string player3 = "", string player4 = "")
        {
            Player p2 = getPlayer(player2);
            Player p3 = getPlayer(player3);
            Player p4 = getPlayer(player4);
            ILobbyCallBack currentPlayer = OperationContext.Current.GetCallbackChannel<ILobbyCallBack>();
            Player player1 = getPlayerCallback(currentPlayer);
            if(player1!=null)
            {
                p2.callbackChannel.InviteToPlay(player1.Username);
                if (p3 != null) { p3.callbackChannel.InviteToPlay(player1.Username); }
                if (p4 != null) { p3.callbackChannel.InviteToPlay(player1.Username); }
            }
          
        }

        public void ReplyToInvite(string player)
        {
            //inform the main player that the invites have been accepted/not
            //start new game with the players that have accepted
            //use callbacks
        }
        ///
        private Player getPlayer(string name)
        {
            foreach (Player p in players)
            { if (p.Username == name) return p; }
            return null;
        }
        private Player getPlayerCallback(ILobbyCallBack cb)
        {
            Player player = null;
            foreach (Player pl in players)
            {
                if (pl.callbackChannel == cb)
                    player = pl;
            }
            return player;
        }

       
        public List<string> getPlayers()
        {
            return initializeConnection.getPlayerList();
        }
        private void ReasignPriority()
        { 
            for (int i=1;i<= players.Count;i++)
            {
                if (players.ElementAt(i-1).Priority != i)
                    players.ElementAt(i-1).Priority = i;
            }
        }
    }
}

