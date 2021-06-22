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
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Window
    {

        private string username;

        private string avatarURL;

        Utils utils = new Utils();

        public EditProfile(string accountUsername)
        {
            InitializeComponent();
            string userD = utils.GetUserData(accountUsername);
            JObject jParser = JObject.Parse(userD);
            string avatarURL = (string)jParser.SelectToken("avatar");

            BitmapImage image = new BitmapImage(new Uri(@avatarURL));
            avatarICON.Source = image;
            username = accountUsername;
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LMBDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if(AvatarBox.Text.Length <= 0)
            {
                Close();
                return;
            }
            if(AvatarBox.Text.Length > 1 && !AvatarBox.Text.Contains("http"))
            {
                MessageBox.Show("The URL is invalid!");
                return;
            }
            avatarURL = AvatarBox.Text;
            var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
            var database = client.GetDatabase("main");
            var collection = database.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var dbData = collection.Find(filter).FirstOrDefault();
            var update = Builders<BsonDocument>.Update.Set("avatar", avatarURL);
            collection.UpdateOne(filter, update);
            ReloadPage();
        }

        private void ReloadPage()
        {
            string userD = utils.GetUserData(username);
            JObject jParser = JObject.Parse(userD);
            string avatarURL = (string)jParser.SelectToken("avatar");

            BitmapImage image = new BitmapImage(new Uri(@avatarURL));
            avatarICON.Source = image;
            username = username;
        }

    }
}
