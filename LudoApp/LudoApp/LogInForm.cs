using LudoApp.proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;

namespace LudoApp
{
    public partial class LogInForm : MetroFramework.Forms.MetroForm
    {
        LudoApp.proxy.AccountClient accProxy;
        Form lobby;
        Form register;
        public LogInForm()
        {
            InitializeComponent();
            accProxy = new AccountClient();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            string k = accProxy.LogIn(metroTextBox2.Text, metroTextBox3.Text);
            if (k != null)
            {
                lobby = new LobbyForm(metroTextBox2.Text);
                this.lobby.Show();
                this.Hide();
            }
            else lbError.Text = "Login failed.";
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            register = new RegistrationForm();
            this.register.Show();
        }
    }
}
