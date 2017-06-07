using System;
using System.Windows.Forms;
using LudoApp.proxy;
using System.ServiceModel;
using System.Linq;

namespace LudoApp
{
    public partial class LobbyForm : MetroFramework.Forms.MetroForm, ILobbyCallback
    {
        LudoApp.proxy.AccountClient proxy;
        proxy.LobbyClient lobby;
        Form lober;
        string[] rankings;
        string playerName;
        string[] players;
        public LobbyForm(string playerName)
        {
            InitializeComponent();
            proxy = new LudoApp.proxy.AccountClient();
            lobby = new proxy.LobbyClient(new InstanceContext(this));
            rankings = proxy.getRanking();
            players = proxy.getPlayers();
            addRanking();
            this.playerName = playerName;
            label1.Text = "Welcome, " + playerName;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            lober = new GameForm(playerName);
            this.lober.Show();
            this.Close();
        }

        public void addRanking()
        {
            for (int i = 0; i < rankings.Length; i++)
            {
                listBox1.Items.Add(rankings[i].ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.lbNewgame.Items.Add(this.lbPlayers.SelectedItem);
        }

        private void btnSendInvite_Click(object sender, EventArgs e)
        {
            var myList = lbNewgame.Items.Cast<String>().ToList();
            if(this.lbNewgame.Items.Count==0 || this.lbNewgame.Items.Count>3)
            MessageBox.Show("You have to add at least one player and maximum 3"); 
            else lobby.InvitePlayers(myList.ElementAt(0),"","");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.lbNewgame.Items.Remove(this.lbNewgame.SelectedItem);
        }
        private void getAllPlayers()
        {
            for (int i = 0; i < players.Length; i++)
            {
              lbPlayers.Items.Add(players[i].ToString());
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.lbPlayers.Items.Clear();
            this.getAllPlayers();
        }

        public void InviteToPlay(string username)
        {
            MessageBox.Show("Invite", "you have been invited by " + username + " to start a game. Accept?", MessageBoxButtons.YesNo);
            if (DialogResult==DialogResult.Yes)
            {
                lober = new GameForm(playerName);
                this.lober.Show();
                this.Hide();
            }
            else MessageBox.Show("Player has deecliend your invte");
        }
    }
}
