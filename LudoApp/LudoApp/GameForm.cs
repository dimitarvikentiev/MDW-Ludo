using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ServiceModel;
using LudoApp.proxy;


namespace LudoApp
{
    public partial class GameForm : Form, IGameCallback
    {
        AccountClient accountProxy;
        GameClient proxy;
        public Player[] players;
        string playerName;
        List<PictureBox> boxes;
        List<PictureBox> winningRedPb;
        List<PictureBox> winningBluePb;
        List<PictureBox> winningYelloPb;
        List<PictureBox> winningGreenPb;
        List<PictureBox> pawnsPictures;
        List<Image> diceNb;
        int Result;
        bool pause = true;
        bool start = false;
        SoundPlayer playTone;
        int turn = 0;

        public GameForm(string playerName)
        {
            InitializeComponent();
            proxy = new GameClient(new InstanceContext(this));
            accountProxy = new AccountClient();
            this.playerName = playerName;
            label4.Text = this.playerName;
            pbDice.Visible = false;
            players = proxy.GetPlayers();
            CreatePlayer(playerName);
            NewPlayerConnected(players);
            DrawPawns();
            boxes = new List<PictureBox>();
            GetPictureBoxes();
            diceNb = new List<Image>();
            addDiceNb();
            playTone = new SoundPlayer();
            getPlayerName();
            this.lbChat.Items.Add("Roll a die to start playing");
            start = true;
        }
        private void kickOut(string player)
        {
            if(players.Count()==4)
            {
                MessageBox.Show("Already enough players");
                LobbyForm lobby = new LobbyForm(player);
                this.Close();
                lobby.Show();
            }
           
        }
        public void addDiceNb()
        {
            Image nb1 = LudoApp.Properties.Resources.Dice1;
            Image nb2 = LudoApp.Properties.Resources.Dice2;
            Image nb3 = LudoApp.Properties.Resources.Dice3;
            Image nb4 = LudoApp.Properties.Resources.Dice4;
            Image nb5 = LudoApp.Properties.Resources.Dice5;
            Image nb6 = LudoApp.Properties.Resources.Dice6;

            diceNb.Add(nb1);
            diceNb.Add(nb2);
            diceNb.Add(nb3);
            diceNb.Add(nb4);
            diceNb.Add(nb5);
            diceNb.Add(nb6);
        }
        public void CreatePlayer(string pName)
        {
            players = proxy.Connect(pName, 1, 1, players.Count()+1);
        }

        private void GetPictureBoxes()
        {
            for (int i = 1; i <= 56; i++)
            {
                foreach (PictureBox pb in this.panel1.Controls.OfType<PictureBox>())
                {
                    if (pb.Name == ("pb" + i))
                        boxes.Add(pb);
                }
            }
            winningRedPb = new List<PictureBox>();
            winningBluePb = new List<PictureBox>();
            winningYelloPb = new List<PictureBox>();
            winningGreenPb = new List<PictureBox>();
            pawnsPictures = new List<PictureBox>();
            for (int i = 1; i <= 5; i++)
            {
                foreach (PictureBox pb in this.panel1.Controls.OfType<PictureBox>())
                {
                    if (pb.Name == ("fbpb" + i))
                        winningBluePb.Add(pb);
                    if (pb.Name == ("fypb" + i))
                        winningYelloPb.Add(pb);
                    if (pb.Name == ("fgpb" + i))
                        winningGreenPb.Add(pb);
                    if (pb.Name == ("frpb" + i))
                        winningRedPb.Add(pb);
                }
            }
           
            foreach (PictureBox pb in this.panel1.Controls.OfType<PictureBox>())
            {
                if (pb.Name.Contains("Paw"))
                {
                    pawnsPictures.Add(pb);
                }
            }
        }

        public void DiceNotify(int result, string playerName)
        {
            foreach (var player in players)
            {
                this.label2.Text = playerName + " rolled: " + result;
                pbDice.Image = diceNb.ElementAt(result - 1);
                pbDice.Visible = true;
                playTone.playSound("die");
            }
            this.Result = result;
        }

        private void btnRollDie_Click(object sender, EventArgs e)
        {
            if (players.Count() <= 1) { MessageBox.Show("You cannot start the game with only one player!"); }
            else if(proxy.diceTurn(playerName) && turn!=1)
                {
                    turn++;
                    int diceResult = proxy.ThrowDice(playerName);
                    Result = diceResult;
                    label2.Text = diceResult.ToString();
                    pbDice.Image = diceNb.ElementAt(diceResult - 1);
                    pbDice.Visible = true;
                    playTone.playSound("die");
                    if (pawnOut() == false && Result != 6)
                    {
                        proxy.setTurn();turn = 0;
                    }
                if (Result == 6) { turn++; }
                }
                else MessageBox.Show("Not your turn!");
            
        }
        private bool pawnOut()
        {
          foreach(Player p in players)
            {
                if(p.Username==playerName)
                { 
                    foreach(Pawn pawn in p.pawns)
                    { if (pawn.currentPosition !=0) return true; }
                }
            }
            return false;
        }
        public void NewPlayerConnected(Player[] p)
        {
            listBox2.Items.Clear();
            this.players = p;
            foreach (var v in players)
            {
                listBox2.Items.Add(v.Username);
                getPlayerName();
            }
            DrawPawns();
            //this.players = proxy.GetPlayers();
        }

        #region Chat sistem
        private void btn_send_Click(object sender, EventArgs e)
        {
            lbChat.Items.Add("[" + DateTime.Now + "]");
            lbChat.Items.Add(playerName + ": " + cbMsg.Text);
            proxy.SendMessage(cbMsg.Text, playerName);
            cbMsg.Text = "";
            playTone.playSound("send");
        }

        public void MessageRecieved(string msg, string playerName)
        {
            lbChat.Items.Add("[" + DateTime.Now + "]");
            lbChat.Items.Add(playerName + ": " + msg);
            playTone.playSound("received");
        }
        #endregion

        private void DrawPawns()
        {
            List<Color> RemainingColors = new List<Color>();
            foreach (Player p in players)
            {

                foreach (Pawn pawn in p.pawns)

                {
                    RemainingColors.Add(pawn.PawnColor);
                    if (pawn.PawnColor == Color.Red)
                    {
                        RedPawn1.Visible = true;
                        RedPawn2.Visible = true;
                        RedPawn3.Visible = true;
                        RedPawn4.Visible = true;

                    }
                    if (pawn.PawnColor == Color.Green)
                    {
                        GreenPawn1.Visible = true;
                        GreenPawn2.Visible = true;
                        GreenPawn3.Visible = true;
                        GreenPawn4.Visible = true;
                    }
                    if (pawn.PawnColor == Color.Blue)
                    {
                        BluePawn1.Visible = true;
                        BluePawn2.Visible = true;
                        BluePawn3.Visible = true;
                        BluePawn4.Visible = true;
                    }
                    if (pawn.PawnColor == Color.Yellow)
                    {
                        YellowPawn1.Visible = true;
                        YellowPawn2.Visible = true;
                        YellowPawn3.Visible = true;
                        YellowPawn4.Visible = true;
                    }
                }
            }
            if (RemainingColors.Contains(Color.Red) == false)
            {
                RedPawn1.Visible = false;
                RedPawn2.Visible = false;
                RedPawn3.Visible = false;
                RedPawn4.Visible = false;
            }
            if (RemainingColors.Contains(Color.Green) == false)
            {
                GreenPawn1.Visible = false;
                GreenPawn2.Visible = false;
                GreenPawn3.Visible = false;
                GreenPawn4.Visible = false;
            }
            if (RemainingColors.Contains(Color.Blue) == false)
            {
                BluePawn1.Visible = false;
                BluePawn2.Visible = false;
                BluePawn3.Visible = false;
                BluePawn4.Visible = false;
            }
            if (RemainingColors.Contains(Color.Yellow) == false)
            {
                YellowPawn1.Visible = false;
                YellowPawn2.Visible = false;
                YellowPawn3.Visible = false;
                YellowPawn4.Visible = false;
            }
            panel1.Invalidate();
        }

        private Pawn getPawn(Color c, int id)
        {
            foreach(Player p in players)
            {
                foreach(Pawn pawn in p.pawns)
                {
                    if(pawn.PawnColor==c && pawn.PawnID==id)
                    {
                        return pawn;
                    }
                }
            }
            return null;
        }

        public void RedrawPawn(Pawn p)
        {
            foreach (Player player in players)
            {
                foreach (Pawn pawn in player.pawns)
                {
                    if (pawn != p && pawn.PawnColor != p.PawnColor && compareLocations(pawn, p))
                    {
                        pawn.currentPosition = 0;
                        PictureBox current = new PictureBox();
                        foreach (PictureBox pb in pawnsPictures)
                        {
                            if (pb.Name.Contains(pawn.PawnID.ToString()) && pb.Name.Contains(pawn.getColorString))
                                current = pb;
                        }
                        current.Location = pawn.InitialLocation;
                        current.Invalidate();
                    }
                }
            }
        }

        public void RespawnPawn(Pawn p)
        {
            bool check = false;
            foreach (Player player in players)
            {
                foreach (Pawn pawn in player.pawns)
                {
                    //check the picture instead of the position
                    if (pawn != p && pawn.PawnColor != p.PawnColor && compareLocations(pawn, p))
                    {
                        pawn.currentPosition = 0;
                        PictureBox current = new PictureBox();
                        foreach (PictureBox pb in pawnsPictures)
                        {
                            if (pb.Name.Contains(pawn.PawnID.ToString()) && pb.Name.Contains(pawn.getColorString))
                            {
                                current = pb;
                                check = true;
                            }
                        }
                        current.Location = pawn.InitialLocation;
                        current.Invalidate();
                    }
                    if (check)
                    {
                        proxy.ResetPawn(p);
                    }
                }
            }
        }

        private bool compareLocations(Pawn pawn1, Pawn pawn2)
        {
            PictureBox pawn1pb = new PictureBox();
            PictureBox pawn2pb = new PictureBox();
            foreach (PictureBox pb in pawnsPictures)
            {
                if (pb.Name.Contains(pawn1.PawnID.ToString()) && pb.Name.Contains(pawn1.getColorString))
                {
                    pawn1pb = pb;
                }
                if (pb.Name.Contains(pawn2.PawnID.ToString()) && pb.Name.Contains(pawn2.getColorString))
                {
                    pawn2pb = pb;
                }
            }
            if (pawn1pb.Location == pawn2pb.Location)
                return true;
            return false;
        }

        public void RespawNotify(Pawn p)
        {
            RedrawPawn(p);
        }

        private void MovePawn(int id, Color color, PictureBox pb)
        {
            int newPosition = 0;
            foreach (var player in players)
            {
                foreach (Pawn pawn in player.pawns)
                {
                    if (pawn.PawnID == id && pawn.PawnColor == color)
                    {
                        if (pawn.currentPosition == 100)
                            this.lbChat.Items.Add("You cannot move that pawn anymore");
                        if (pawn.currentPosition + Result > pawn.StartPosition && pawn.currentPosition < pawn.StartPosition && pawn.currentPosition != 0 && pawn.PawnColor == Color.Red || pawn.Finished)
                        {
                            newPosition = pawn.currentPosition + Result - pawn.StartPosition;
                            if (pawn.Finished)
                                newPosition = pawn.currentPosition + Result;
                            pawn.currentPosition = newPosition;
                            getFinishPicture(color, pb, pawn);
                        }
                        else if (pawn.PawnColor == Color.Green && pawn.currentPosition + Result > boxes.Count || pawn.Finished)
                        {
                            newPosition = Result - (boxes.Count - pawn.currentPosition);
                            if (pawn.Finished)
                                newPosition = pawn.currentPosition + Result;
                            pawn.currentPosition = newPosition;
                            getFinishPicture(color, pb, pawn);
                        }
                        else if (pawn.PawnColor == Color.Yellow && pawn.StartPosition - pawn.currentPosition < 6 && pawn.StartPosition - pawn.currentPosition > 0 || pawn.Finished)
                        {
                            newPosition = pawn.currentPosition + Result - pawn.StartPosition;
                            if (pawn.Finished)
                                newPosition = pawn.currentPosition + Result;
                            pawn.currentPosition = newPosition;
                            getFinishPicture(color, pb, pawn);
                        }
                        else
                        {
                            if (pawn.currentPosition != 0)
                            {
                                pawn.currentPosition += Result;
                                getPicturePosition(pawn, pb);
                            }
                            if (pawn.currentPosition == 0 && Result == 6)
                            {
                                pawn.currentPosition = pawn.StartPosition;
                                getPicturePosition(pawn, pb);
                            }
                        }
                        proxy.MoveForward(pawn, pawn.currentPosition);
                        RespawnPawn(pawn);
                    }
                }
            }
            if (proxy.AllFinish(playerName))
            {
                MessageBox.Show("Congratulations, you win!");
                proxy.NotifyForWinner(playerName);
                playTone.playSound("win");
                //Close();
            }}

        private void SetNewPawnPosition(int id, Color color, PictureBox pb)
        {
            int newPosition = 0;
            foreach (var player in players)
            {
                foreach (Pawn pawn in player.pawns)
                {
                    if (pawn.PawnID == id && pawn.PawnColor == color)
                    {
                        if (pawn.currentPosition + Result > pawn.StartPosition && pawn.currentPosition < pawn.StartPosition && pawn.currentPosition != 0 && pawn.PawnColor != Color.Green && pawn.PawnColor != Color.Yellow || pawn.Finished)
                        {
                            newPosition = pawn.currentPosition + Result - pawn.StartPosition;
                            if (pawn.Finished)
                                newPosition = pawn.currentPosition + Result;
                            pawn.currentPosition = newPosition;
                            getFinishPicture(color, pb, pawn);
                        }
                        else if (pawn.PawnColor == Color.Green && pawn.currentPosition + Result > boxes.Count || pawn.Finished)
                        {
                            newPosition = Result - (boxes.Count - pawn.currentPosition);
                            if (pawn.Finished)
                                newPosition = pawn.currentPosition + Result;
                            pawn.currentPosition = newPosition;
                            getFinishPicture(color, pb, pawn);
                        }
                        else if (pawn.PawnColor == Color.Yellow && pawn.StartPosition - pawn.currentPosition < 6 && pawn.StartPosition - pawn.currentPosition > 0 || pawn.Finished)
                        {
                            newPosition = pawn.currentPosition + Result - pawn.StartPosition;
                            if (pawn.Finished)
                                newPosition = pawn.currentPosition + Result;
                            pawn.currentPosition = newPosition;
                            getFinishPicture(color, pb, pawn);
                        }
                        else if (pawn.PawnColor == Color.Blue && pawn.StartPosition - pawn.currentPosition < 6 && pawn.StartPosition - pawn.currentPosition > 0 || pawn.Finished)
                        {
                            newPosition = pawn.currentPosition + Result - pawn.StartPosition;
                            if (pawn.Finished)
                                newPosition = pawn.currentPosition + Result;
                            pawn.currentPosition = newPosition;
                            getFinishPicture(color, pb, pawn);
                        }
                        else
                        {
                            if (pawn.currentPosition != 0)
                            {
                                pawn.currentPosition += Result;
                                getPicturePosition(pawn, pb);
                            }
                            else if (pawn.currentPosition == 0 && Result == 6)
                            {
                                pawn.currentPosition = pawn.StartPosition;
                                getPicturePosition(pawn, pb);
                            }
                        }
                    }
                }
            }
        }

        public void PawnNotify(Pawn p, int newPosition)
        {
            PictureBox current = new PictureBox();
            foreach (PictureBox pb in pawnsPictures)
            {
                if (pb.Name.Contains(p.PawnID.ToString()) && pb.Name.Contains(p.getColorString))
                    current = pb;
            }
            p.currentPosition = newPosition;
            SetNewPawnPosition(p.PawnID, p.PawnColor, current);
        }

        private void getFinishPicture(Color c, PictureBox pb, Pawn p)
        {
            if (c == Color.Red)
            {
                if (p.currentPosition > winningRedPb.Count && p.AllFinish==false)
                {
                    int newPosition = -p.currentPosition + winningRedPb.Count;
                    p.currentPosition = 100;
                    proxy.PawnHasFinish(p, playerName);
                    p.AllFinish = true;

                    if (p.PawnID == 1)
                        pb.Location = new Point(247,209);
                    if (p.PawnID == 2)
                        pb.Location = new Point(231,233);
                    if (p.PawnID == 3)
                        pb.Location = new Point(247,237);
                    if (p.PawnID == 4)
                        pb.Location = new Point(247,257);
                }
                else { pb.Location = winningRedPb.ElementAt(p.currentPosition).Location; p.Finished = true; }
                pb.Invalidate();

            }
            if (c == Color.Green)
            {
                if (p.currentPosition > winningGreenPb.Count && p.AllFinish == false)
                {
                    int newPosition = -p.currentPosition + winningGreenPb.Count;
                    p.currentPosition = 100;
                    proxy.PawnHasFinish(p,playerName);
                    if (p.PawnID == 1)
                        pb.Location = new Point(212,247);
                    if (p.PawnID == 2)
                        pb.Location = new Point(213,267);
                    if (p.PawnID == 3)
                        pb.Location = new Point(189,269);
                    if (p.PawnID == 4)
                        pb.Location = new Point(237,269);
                }
                else
                {
                    foreach (PictureBox picture in winningGreenPb)
                    {
                        if (picture.Name.Contains(p.currentPosition.ToString()))
                            pb.Location = picture.Location; p.Finished = true;
                    }
                    pb.Invalidate(); ;
                }
            }
            if (c == Color.Yellow)
            {
                if (p.currentPosition > winningYelloPb.Count && p.AllFinish == false)
                {
                    int newPosition = -p.currentPosition + winningYelloPb.Count;
                    p.currentPosition = 100;
                    proxy.PawnHasFinish(p, playerName);
                    p.AllFinish = true;
                    if (p.PawnID == 1)
                        pb.Location = new Point(182,209);
                    if (p.PawnID == 2)
                        pb.Location = new Point(182,257);
                    if (p.PawnID == 3)
                        pb.Location = new Point(200,233);
                    if (p.PawnID == 4)
                        pb.Location = new Point(182,233);
                }
                else { pb.Location = winningYelloPb.ElementAt(p.currentPosition).Location; p.Finished = true; }
                pb.Invalidate();
            }

            if (c == Color.Blue)
            {
                if (p.currentPosition > winningBluePb.Count && p.AllFinish == false)
                {
                    int newPosition = -p.currentPosition + winningRedPb.Count;
                    p.currentPosition = 100;
                    proxy.PawnHasFinish(p, playerName);
                    p.AllFinish = true;
                    if (p.PawnID == 1)
                        pb.Location = new Point(218,219);
                    if (p.PawnID == 2)
                        pb.Location = new Point(208,195);
                    if (p.PawnID == 3)
                        pb.Location = new Point(189,195);
                    if (p.PawnID == 4)
                        pb.Location = new Point(232,198);
                }
                else { pb.Location = winningBluePb.ElementAt(p.currentPosition).Location; p.Finished = true; }
                pb.Invalidate();
            }

        }

        private void getPicturePosition(Pawn p, PictureBox pb)
        {   
            if (p.currentPosition >= boxes.Count)
            {
                int newPosition = p.currentPosition - boxes.Count;
                pb.Location = boxes.ElementAt(newPosition).Location;
                p.currentPosition = newPosition;
            }
            else
            {       pb.Location = boxes.ElementAt(p.currentPosition).Location;
                    pb.Invalidate();
            }      
        }

        #region start, pause and resume functions
        private void Pause(bool b)
        {
            if (b)
            {
                if (!start)
                {
                    btn_startPauseResume.Text = "PAUSE";
                    panel1.Enabled = true;
                    btnRollDie.Enabled = true;
                    lbChat.Items.Add("The game is started!");
                }
                else
                {
                    btn_startPauseResume.Text = "PAUSE";
                    panel1.Enabled = true;
                    btnRollDie.Enabled = true;
                    lbChat.Items.Add("The game is resumed!");
                }
            }
            else
            {
                btn_startPauseResume.Text = "RESUME";
                panel1.Enabled = false;
                btnRollDie.Enabled = false;
                lbChat.Items.Add("The game is paused!");
                start = true;
            }
        }
        public void GamePaused(bool b)
        {
            Pause(b);
            pause = !pause;
            if (b) btn_startPauseResume.Enabled = true;
            else btn_startPauseResume.Enabled = false;
        }
        #endregion

        #region Disconnect a player
        private void btn_quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            proxy.RemovePlayer(playerName);
            proxy.Disconnect();
        }
        #endregion

        private void RedPawn3_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Red, 3);
            if (proxy.getTurn(playerName, p))
            {       if (Result != 6)
                        proxy.setTurn();
              MovePawn(3, Color.Red, this.RedPawn3);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void RedPawn1_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Red, 1);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(1, Color.Red, this.RedPawn1);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn!");
        }
        private void GreenPawn2_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Green, 2);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                playTone.playSound("move");
                MovePawn(2, Color.Green, this.GreenPawn2);
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void GreenPawn1_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Green, 1);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                playTone.playSound("move");
                MovePawn(1, Color.Green, this.GreenPawn1);
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void YellowPawn1_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Yellow, 1);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(1, Color.Yellow, this.YellowPawn1);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void YellowPawn2_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Yellow, 2);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                playTone.playSound("move");
                MovePawn(2, Color.Yellow, this.YellowPawn2);
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void YellowPawn3_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Yellow, 3);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(3, Color.Yellow, this.YellowPawn3);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void YellowPawn4_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Yellow, 4);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(4, Color.Yellow, this.YellowPawn4);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void BluePawn4_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Blue, 4);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(4, Color.Blue, this.BluePawn4);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void BluePawn1_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Blue, 1);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(1, Color.Blue, this.BluePawn1);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void BluePawn2_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Blue, 2);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(2, Color.Blue, this.BluePawn2);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void BluePawn3_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Blue, 3);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(3, Color.Blue, this.BluePawn3);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void pbGreenPawn4_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Green, 4);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(4, Color.Green, this.GreenPawn4);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void pbRedPawn2_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Red, 2);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(2, Color.Red, this.RedPawn2);
                playTone.playSound("move");

            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void GreenPawn3_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Green, 3);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(3, Color.Green, this.GreenPawn3);
                playTone.playSound("move");

            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void RedPawn4_Click(object sender, EventArgs e)
        {
            Pawn p = getPawn(Color.Red, 4);
            if (proxy.getTurn(playerName, p))
            {
                if (Result != 6)
                    proxy.setTurn();
                MovePawn(4, Color.Red, this.RedPawn4);
                playTone.playSound("move");
            }
            else MessageBox.Show("Not your turn/pawn!");
        }
        private void CheckLocation(int location, Pawn currentPawn)
        {
            foreach (Player p in players)
            {
                foreach (Pawn pawn in p.pawns)
                {
                    if (pawn.currentPosition == location && pawn != currentPawn)
                    {
                        foreach (PictureBox pb in pawnsPictures)
                        {
                            if (pb.Location == boxes.ElementAt(pawn.currentPosition).Location && pb.Name.Contains(pawn.PawnColor.ToString()) && pb.Name.Contains(pawn.PawnID.ToString())) ;
                            {
                                pb.Location = pawn.InitialLocation;
                                boxes.ElementAt(pawn.currentPosition).Invalidate();
                                pb.Invalidate();
                            }
                        }
                        pawn.currentPosition = 0;
                    }
                }
            }

        }
        public void NewWinner(string username)
        {
            lbChat.Items.Add("[" + DateTime.Now + "]");
            lbChat.Items.Add("System: " + username + " finished the game!");
        }

        private void getPlayerName()
        {
            lbPlayer1.Text = "";
            lbPlayer2.Text = "";
            lbPlayer3.Text = "";
            lbPlayer4.Text = "";
            foreach (Player player in players)
            {
                if (player.Priority == 1)
                    lbPlayer1.Text = player.Username;
                if (player.Priority == 2)
                    lbPlayer2.Text = player.Username;
                if (player.Priority == 3)
                    lbPlayer3.Text = player.Username;
                if (player.Priority == 4)
                    lbPlayer4.Text = player.Username;
            }
        }


        private void btn_startPauseResume_Click_1(object sender, EventArgs e)
        { 
                Pause(pause);
                proxy.GamePause(pause);
                pause = !pause;
          
        }
    }
}

