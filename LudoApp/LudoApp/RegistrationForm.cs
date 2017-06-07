using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LudoApp
{
    public partial class RegistrationForm : MetroFramework.Forms.MetroForm
    {
        Form game;
        LudoApp.proxy.AccountClient proxy;
        public RegistrationForm()
        {
            InitializeComponent();
            proxy = new LudoApp.proxy.AccountClient();
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            bool create = proxy.createAccount(metroTextBox1.Text, metroTextBox2.Text, tbPassword.Text);
            if(create == true)
            {
                game = new LobbyForm(metroTextBox1.Text);
                this.game.Show();
                this.Hide();
            }
            else MessageBox.Show("Sorry, something went wrong. Try again later!");

        }
    }
}
