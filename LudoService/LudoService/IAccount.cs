using System.Collections.Generic;
using System.ServiceModel;

namespace LudoService
{
    [ServiceContract]
    public interface IAccount
    {
        [OperationContract]
        string LogIn(string username, string password);

        [OperationContract]
        bool createAccount(string name, string username, string password);

        [OperationContract]
        List<string> getRanking();

        [OperationContract]
        bool AwardWinner(string userName, int points);

        [OperationContract]
        List<string> getPlayers();
    }
}
