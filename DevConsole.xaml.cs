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
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class DevConsole : Window
    {
        public string newPass;

        private bool isDisabled = false;

        Utils utils = new Utils();

        public DevConsole()
        {
            InitializeComponent();
        }

        private void ToggleCheat(object sender, RoutedEventArgs e)
        {
            var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
            var database = client.GetDatabase("main");
            var collection = database.GetCollection<BsonDocument>("utils");
            var filter = Builders<BsonDocument>.Filter.Eq("data_id", 9902);
            var dbData = collection.Find(filter).FirstOrDefault();
            if (!isDisabled)
            {
                var update = Builders<BsonDocument>.Update.Set("disabled", true);
                collection.UpdateOne(filter, update);
                isDisabled = true;
                MessageBox.Show("Cheat has been toggled off!");
            } else if (isDisabled)
            {
                var update = Builders<BsonDocument>.Update.Set("disabled", false);
                collection.UpdateOne(filter, update);
                isDisabled = false;
                MessageBox.Show("Cheat has been toggled on!");
            }
        }

        private void UpdatePassword(object sender, RoutedEventArgs e)
        {
            if (KeyBox.Text != null)
            {
                newPass = KeyBox.Text;
            } else
            {
                MessageBox.Show("Password can't be null!");
            }

            var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
            var database = client.GetDatabase("main");
            var collection = database.GetCollection<BsonDocument>("utils");
            var filter = Builders<BsonDocument>.Filter.Eq("data_id", 9902);
            var dbData = collection.Find(filter).FirstOrDefault();
            var update = Builders<BsonDocument>.Update.Set("password", newPass);
            collection.UpdateOne(filter, update);
            MessageBox.Show("Password updated succesfully!");
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
