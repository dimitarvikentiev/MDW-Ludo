using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LudoService
{
    [DataContract]
    class Game
    {
        public Player firstPlayer;
        public Player secondPlayer;
        public Player thirdPlayer;
        public Player fourthPlayer;
        public Player FirstWinner;
        public Player SecondWinner;
        public Player ThirdWinner;
        public Player FourthWinner;

        public Game(Player player1, Player player2, Player player3=null, Player player4=null)
        {
            firstPlayer = player1;
            secondPlayer = player2;
            thirdPlayer = player3;
            fourthPlayer = player4;
        }

        public int SetWinner(Player p)
        {
            if (FirstWinner == null)
            {
                FirstWinner = p;
                return 3;
            }
            else if (FirstWinner != null && SecondWinner == null)
            {
                SecondWinner = p;
                return 2;
            }
            else if (FirstWinner != null && SecondWinner != null && ThirdWinner == null)
            {
                ThirdWinner = p;
                return 1;
            }
            else if(FirstWinner != null && SecondWinner != null && ThirdWinner != null && FourthWinner == null)
            {
                FourthWinner = p;
                return 0;
            }
            return 0;
        }
    }
}
