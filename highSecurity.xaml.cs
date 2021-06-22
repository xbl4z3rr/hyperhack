using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HyperHack
{
    public partial class highSecurity : Window
    {

        Utils utils = new Utils();

        public string keyCode;

        public string prevPass;

        private string email;

        public highSecurity(string pass, string accountEmail)
        {
            InitializeComponent();
            email = accountEmail;
            string data = utils.GetManifest();
            JObject jObject = JObject.Parse(data);
            string version = (string)jObject.SelectToken("version");
            bool disabled = (bool)jObject.SelectToken("disabled");
            string opt_version = (string)jObject.SelectToken("opt-version");

            prevPass = pass;


            if ((string)versionLabel.Content == version || (string)versionLabel.Content == opt_version)
            {
                return;
            }
            else
            {
                MessageBox.Show("Error: Out Of Date", "You are using an outdated version! Please download the new version to continue using this app.");
                Close();
            }

            if (disabled)
            {
                MessageBox.Show("CHEAT IS DISABLED!");
                Close();
            }
        }

        public void Continue(object sender, RoutedEventArgs e)
        {
            string Key = KeyBox.Text;
            ValidateKey(Key, prevPass);
        }

        private void ValidateKey(string Key, string pass)
        {
            string data = utils.GetManifest();
            JObject jObject = JObject.Parse(data);
            string code = (string)jObject.SelectToken("digitCode");

            if (Key != code)
            {
                MessageBox.Show("INVALID CODE!");
            }
            else if (Key == code)
            {
                keyCode = Key;
                MainWindow main = new MainWindow(pass, email);
                Close();
                main.Show();
            }
        }
        private void LMBDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
