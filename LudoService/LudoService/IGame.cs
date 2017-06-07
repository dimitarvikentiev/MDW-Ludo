using System.Collections.Generic;
using System.ServiceModel;

namespace LudoService
{
    [ServiceContract(CallbackContract = typeof(IGameCallback))]
    public interface IGame
    {
        [OperationContract]
        List<Player> Connect(string username, int nbOfPoints, int Position, int priority);

        [OperationContract(IsOneWay = true)]
        void Disconnect();

        [OperationContract]
        void CreatePlayer(string username, int nbOfPoints, int position, int priority);

        [OperationContract]
        void RemovePlayer(string playerName);

        [OperationContract]
        List<Player> GetPlayers();

        [OperationContract]
        string GetPlayerInfo(string playerName);

        [OperationContract]
        int ThrowDice(string playerName);

        [OperationContract]
        void MoveForward(Pawn p, int newPosition);

        [OperationContract]
        void SendMessage(string msg, string playerName);

        [OperationContract]
        void GamePause(bool b);

        [OperationContract]
        void ResetPawn(Pawn p);

        [OperationContract]
        bool AllFinish(string playerName);

        [OperationContract(IsOneWay = true)]
        void PawnHasFinish(Pawn pawn, string playerName);

        [OperationContract]
        void NotifyForWinner(string playerName);
        [OperationContract]
        void setTurn();
        [OperationContract]
        bool getTurn(string player, Pawn pawn);
        [OperationContract]
        bool diceTurn(string player);
    }
  



    }
