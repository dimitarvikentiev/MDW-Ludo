using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LudoService
{
    [ServiceContract(CallbackContract = typeof(ILobbyCallBack))]
    public interface ILobby
    {
        [OperationContract]
        void InvitePlayers(string player2, string player3 = "", string player4 = "");   
    }
}
