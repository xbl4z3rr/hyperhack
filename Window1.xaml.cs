// QuizizzSupport.Window1
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using HyperHack;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using System.Net;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using DeviceId;

namespace HyperHack
{
	public partial class Window1 : Window
	{
        Utils utils = new Utils();

		public static string GetDeviceID()
		{
			return new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().ToString();
		}

		public Window1()
		{
			InitializeComponent();

			//Normal checks
			string data = utils.GetManifest();
			JObject BSONParser = JObject.Parse(data);
			string version = (string)BSONParser.SelectToken("version");
			bool disabled = (bool)BSONParser.SelectToken("disabled");
			string opt_version = (string)BSONParser.SelectToken("opt-version");

			if ((string)versionLabel.Content == version || (string)versionLabel.Content == opt_version)
			{
				return;
			}
			else
			{
				MessageBox.Show("Error: Out Of Date", "You are using an outdated version! Please download the new version to continue using this app.");
				Close();
			}
		}

		private bool ValidateAccount(string email, string password)
        {
			//Get Device ID
			string deviceID = GetDeviceID();

            //MognoDB Data
            var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
            var database = client.GetDatabase("main");
			var collection = database.GetCollection<BsonDocument>("users");
			var filter = Builders<BsonDocument>.Filter.Eq("username", EmailBox.Text);
			var dbData = collection.Find(filter).FirstOrDefault();

            if (dbData == null)
            {
                MessageBox.Show("There is no account for this email!");
                return false;
            }
            var data = dbData.ToString();
            data = data.Replace("_", "");
            data = data.Remove(9, 36);

			if (PassBox.Text != password || EmailBox.Text != email || PassBox.Text != password && EmailBox.Text != email) return false;
			return true;
		}

		private void Login(object sender, RoutedEventArgs e)
		{
			string ServerPass = ServerPassBox.Text;
			string AccPass = PassBox.Text;
			string AccEmail = EmailBox.Text;
			bool result = ValidateAccount(AccEmail, AccPass);
			if (result)
			{
				ValidatePass(ServerPass, AccEmail);
			} else
            {
				return;
            }
		}

		private void ValidatePass(string ServerPass, string accountEmail)
        {
			string data = utils.GetManifest();
			JObject jObject = JObject.Parse(data);
			string password = (string)jObject.SelectToken("password");
			bool high_security = (bool)jObject.SelectToken("highSecurity");

			Console.WriteLine(ServerPass);
			Console.WriteLine(password);

			if (ServerPass != password)
            {
				MessageBox.Show("Error: Invalid Creditentials", "The password you entered is incorrect!");
			}
			else if(ServerPass == password)
            {
				if (high_security)
				{
					highSecurity hs = new highSecurity(ServerPass, accountEmail);
					Close();
					MessageBox.Show("INFO: High Security Enabled", "High Security is enabled. You must perform 2FA authentication.");
					hs.Show();
				}
				else if (!high_security)
				{
					MainWindow main = new MainWindow(ServerPass, accountEmail);
					Close();
					main.Show();
				}
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

		private void Register(object sender, RoutedEventArgs e)
        {
			Register reg = new Register();
			reg.Show();
			Close();
        }
	}
}