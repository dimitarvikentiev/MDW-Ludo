using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LudoService
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public int NbOfPoints { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public int NbOfPawns { get; set; }

       

        [DataMember]
        public List<Pawn> pawns { get; set; }

        public Player(string username, int nbOfPoints, int position, int priority)
        {
            this.Username = username;
            this.NbOfPoints = nbOfPoints;
            this.Position = position;
            this.Priority = priority;
            this.NbOfPawns =4;
           
        }
        public ILobbyCallBack callbackChannel { get; set; }
    }
}
