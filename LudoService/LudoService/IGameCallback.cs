using System.Collections.Generic;
using System.ServiceModel;

namespace LudoService
{
    public interface IGameCallback
    {
        [OperationContract(IsOneWay = true)]
        void NewPlayerConnected(List<Player> players);

        [OperationContract]
        void NewWinner(string username);

        [OperationContract(IsOneWay = true)]
        void MessageRecieved(string msg, string playerName);
        
        [OperationContract]
        void DiceNotify(int result, string playerName);

        [OperationContract(IsOneWay = true)]
        void PawnNotify(Pawn p, int newPosition);

        [OperationContract]
        void GamePaused(bool b);

        [OperationContract(IsOneWay = true)]
        void RespawNotify(Pawn p);
      
    }
}
