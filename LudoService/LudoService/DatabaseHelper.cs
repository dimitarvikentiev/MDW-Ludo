using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace LudoService
{
    [DataContract]
    public class DatabaseHelper
    {
        [DataMember]
        public MySqlConnection connection;
        public DatabaseHelper()
        {
            // Establishing connection with the Athena database.
            string connectionInfo = "server=athena01.fhict.local;" +
                                      "database=dbi319356;" +
                                     "user id=dbi319356;" +
                                     "password= T2Xq7JqkHt;" +
                                     "connect timeout=30;";
            this.connection = new MySqlConnection(connectionInfo);
        }
        public bool Award(string userName, int points) {
            bool v = false;
            String sql = "update player_info SET points = points + " + points + " WHERE username ='" + userName + "';";
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                connection.Open();
                command.ExecuteScalar();
                v = true;
            }
            catch
            {
                v = false;
            }
            finally { connection.Close(); }
            return v;
        }
        public object MessageBox { get; private set; }
        public List<string> getRankingDB()
        {
            //List<Ranking> rankings = new List<Ranking>();
            List<string> rankings = new List<string>();
            String sql = "select Username, Points FROM player_info";
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                //foreach (string r in rankings)
                // {
                while (reader.Read())
                {
                    rankings.Add(reader[1].ToString() + " - " + reader[0].ToString());
                }
                // }
                return rankings;


            }
            catch
            {
                return null;
            }
            finally { connection.Close(); }
        }
        public List<string> getPlayerList()
        {
            //List<Ranking> rankings = new List<Ranking>();
            List<string> players = new List<string>();
            String sql = "select Username FROM player_info";
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
            
                while (reader.Read())
                {
                    players.Add(reader[0].ToString());
                }
                // }
                return players;


            }
            catch
            {
                return null;
            }
            finally { connection.Close(); }
        }
        public bool createAccount(string name, string username, string password)
        {
            String sql = "SELECT Count(*) FROM player_info where Username='" + username + "'";
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                connection.Open();
                int user = Convert.ToInt32(command.ExecuteScalar());
                if (user == 0)
                {
                    String insert = "INSERT INTO player_info(Username, Name, Password, Points) VALUES ('" + username + "','" + name + "','" + password + "',0)";
                    MySqlCommand insertcommand = new MySqlCommand(insert, connection);
                    insertcommand.ExecuteNonQuery();
                  
                    return true;
                }
            }
            catch
            {
                return false;

            }
            finally
            {

                connection.Close();
            }
            return false;
        }
        public string logIn(string username, string password)
        {

            String sql = "SELECT Count(*) FROM player_info where Username='" + username + "' AND Password='" + password + "'";
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
               connection.Open();
                int user = Convert.ToInt32(command.ExecuteScalar());
                if (user != 0)
                {
                    String name = "Select Name from player_info where Username='" + username + "'";
                    MySqlCommand getCommand = new MySqlCommand(name, connection);
                    getCommand.ExecuteReader();
                    return name;
                }
                else { return null; }
                

            }
            catch
            {
                return null;

            }
            finally
            {

                connection.Close();
            }
  
        }
    }
}
