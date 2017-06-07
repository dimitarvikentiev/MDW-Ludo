using System.Drawing;
using System.Runtime.Serialization;

namespace LudoService
{
    [DataContract]
    public class Pawn
    {
        [DataMember]
        public int PawnID { get; set; }
        [DataMember]
        public Point InitialLocation { get; set; }
        [DataMember]

        public int StartPosition;
        [DataMember]
        public int EndPosition;
        [DataMember]
        public bool Finished { get; set; }
        [DataMember]
        public int currentPosition { get; set; }
        [DataMember]
        public Color PawnColor { get; set; }
        [DataMember]
        public string getColorString { get; set; }
        [DataMember]
        public Player AttachedPlayer { get; set; }
        [DataMember]
        public bool AllFinish { get; set; }

        public Pawn(int pawnID, Player player,bool finished = false)
        {
            this.PawnID = pawnID;
            this.Finished = finished;
            currentPosition = 0;
            getColor(player.Priority);
            AllFinish = false;
           
           
        }
        private void  getColor(int playerID)
        {
            if (playerID == 1)
            {   PawnColor = Color.Red;
                getColorString = "Red";

                StartPosition = 15;
                EndPosition = 14;
                if(PawnID==1)
                {
                    InitialLocation = new Point(300, 103);
                }
                if (PawnID == 2)
                {
                    InitialLocation = new Point(340, 120);
                }
                if (PawnID == 3)
                {
                    InitialLocation = new Point(319, 129);
                }
                if (PawnID == 4)
                {
                    InitialLocation = new Point(319, 84);
                }
            }
            if (playerID == 2) {

                PawnColor = Color.Green;
                getColorString = "Green";

                StartPosition = 1;
                EndPosition = 56;
                if (PawnID == 1)
                {
                    InitialLocation = new Point(340, 361);
                }
                if (PawnID == 2)
                {
                    InitialLocation = new Point(319, 370);
                }
                if (PawnID == 3)
                {
                    InitialLocation = new Point(300, 354);
                }
                if (PawnID == 4)
                {
                    InitialLocation = new Point(319, 330);
                }
            }
            if (playerID == 3) {

                PawnColor = Color.Yellow;
                getColorString = "Yellow";

                StartPosition = 43;
                EndPosition = 42;
                if (PawnID == 1)
                {
                    InitialLocation = new Point(106, 337);
                }
                if (PawnID == 2)
                {
                    InitialLocation = new Point(127, 368);
                }
                if (PawnID == 3)
                {
                    InitialLocation = new Point(106, 377);
                }
                if (PawnID == 4)
                {
                    InitialLocation = new Point(87, 361);
                }
            }
            if (playerID == 4)
            { PawnColor = Color.Blue;
                getColorString = "Blue";
                StartPosition = 29;
                EndPosition = 28;
                if (PawnID == 1)
                {
                    InitialLocation = new Point(127, 120);
                }
                if (PawnID == 2)
                {
                    InitialLocation = new Point(106, 129);
                }
                if (PawnID == 3)
                {
                    InitialLocation = new Point(87, 113);
                }
                if (PawnID == 4)
                {
                    InitialLocation = new Point(106, 89);
                }   
            } 
        }
    }
}
