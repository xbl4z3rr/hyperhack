using DeviceId;
using MongoDB.Bson;
using MongoDB.Driver;
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
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
		Utils utils = new Utils();

		public static string GetDeviceID()
		{
			return new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().ToString();
		}

		public Register()
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

		private void RegisterAcc(object sender, RoutedEventArgs e)
        {
			var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
			var database = client.GetDatabase("main");
			var collection = database.GetCollection<BsonDocument>("users");
			var filter = Builders<BsonDocument>.Filter.Eq("username", EmBox.Text);
			var dbData = collection.Find(filter).FirstOrDefault();

			if (dbData != null)
			{
				MessageBox.Show("There already is an account with that username!");
				return;
			}

			if(EmBox.Text.Contains(" "))
			{
				MessageBox.Show("Invalid Username!");
				return;
			}

			if(PassBox.Text.Length < 8)
            {
				MessageBox.Show("Your password must be at least 8 characters.");
				return;
            }

			//Data Scheme
			string deviceID = GetDeviceID();
			var userData = new BsonDocument
			{
				{ "user_id", deviceID },
				{ "username", EmBox.Text },
				{ "password", PassBox.Text },
				{ "avatar", "http://cdn.onlinewebfonts.com/svg/img_132120.png" },
				{ "approved", false },
				{ "restricted", false },
				{ "isDev", false },
				{ "isOnlide", false }
			};

			collection.InsertOne(userData);
			MessageBox.Show("Successfully registered your account! Redirecting to login!");
			Window1 window1 = new Window1();
			window1.Show();
			Close();
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
