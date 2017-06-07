using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LudoService
{
    public interface ILobbyCallBack
    {
        [OperationContract]
        void InviteToPlay(string username);
    }
}
