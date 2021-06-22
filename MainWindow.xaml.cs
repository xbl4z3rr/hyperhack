// QuizizzSupport.MainWindow
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HyperHack;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using WpfMath;
using DeviceId;

namespace HyperHack
{
	public partial class MainWindow : Window
	{
        private static readonly HttpClient client = new HttpClient();

		private Structs.KeyInformation KeyInfo;

		private int QuizzesScanned;

		private int PremiumScanLimit = 5;

		private string username;

		internal Button button2;

		Utils utils = new Utils();

		Window1 win1 = new Window1();

		DevConsole console = new DevConsole();

		public string oldPass;

		private bool isConsoleOpen = false;

		private Utils.QuizData CurrentQuiz { get; set; }

		System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromSeconds(2).TotalMilliseconds);

		public bool inMainMenu = false;

		public MainWindow(string pass, string accountUsername)
		{
			//Initialize
			InitializeComponent();
			inMainMenu = true;
			username = accountUsername;


			//MongoDB for Online Users
			var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
			var database = client.GetDatabase("main");
			var collection = database.GetCollection<BsonDocument>("utils");
			var filter = Builders<BsonDocument>.Filter.Eq("data_id", 9902);
			var dbData = collection.Find(filter).FirstOrDefault();
			string data = utils.GetManifest();
			JObject jObject = JObject.Parse(data);
			int onlineUsers = (int)jObject.SelectToken("onlineUsers");
			var update = Builders<BsonDocument>.Update.Set("onlineUsers", onlineUsers + 1);
			collection.UpdateOne(filter, update);
			onlineCount.Content = onlineUsers.ToString();

			oldPass = pass;

			string userD = utils.GetUserData(username);
			JObject jParser = JObject.Parse(userD);
			string avatarURL = (string)jParser.SelectToken("avatar");
			bool isOnline = (bool)jParser.SelectToken("isOnline");
			if (isOnline) {
				MessageBox.Show("There is someone online on this account. If you think this is an error plase contact an admin!");
				var updateye = Builders<BsonDocument>.Update.Set("onlineUsers", onlineUsers - 1);
				collection.UpdateOne(filter, updateye);
				timer.Stop();
				Close();
				return;
			}

			BitmapImage image = new BitmapImage(new Uri(@avatarURL));
			avatarICON.Source = image;

			usernameBTN.Content = username;

			utils.SetStatus(accountUsername, "online");

			//Checking...
			timer.AutoReset = true;
			timer.Elapsed += new System.Timers.ElapsedEventHandler(Checks);
			timer.Start();
		}

		private void Checks(object sender, ElapsedEventArgs e)
		{
			//MongoDB Connect
			var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
			var database = client.GetDatabase("main");
			var collection = database.GetCollection<BsonDocument>("utils");
			var filter = Builders<BsonDocument>.Filter.Eq("data_id", 9902);

			//Getting data
			string apiData = utils.GetManifest();
			JObject apiObject = JObject.Parse(apiData);
			int onlinePeeps = (int)apiObject.SelectToken("onlineUsers");
			string version = (string)apiObject.SelectToken("version");
			string opt_version = (string)apiObject.SelectToken("opt-version");
			bool disabled = (bool)apiObject.SelectToken("disabled");
			bool answersDisabled = (bool)apiObject.SelectToken("answersDisabled");
			bool highSec = (bool)apiObject.SelectToken("highSecurity");
			string password = (string)apiObject.SelectToken("password");
			string code = (string)apiObject.SelectToken("digitCode");

			var deviceID = GetDeviceID();

			string userD = utils.GetUserData(username);
			JObject jParser = JObject.Parse(userD);
			bool restricted = (bool)jParser.SelectToken("restricted");
			bool approved = (bool)jParser.SelectToken("approved");
			string avatarURL = (string)jParser.SelectToken("avatar");

			//Making Changes
			this.Dispatcher.Invoke(() =>
			{

				BitmapImage image = new BitmapImage(new Uri(@avatarURL));
				avatarICON.Source = image;

				usernameBTN.Content = username;

				//Pass Changes
				if (oldPass != password)
				{
					if (isConsoleOpen)
                    {
						return;
                    }
					timer.Stop();
					MessageBox.Show("Error: Password Changed", "The password has been changed. Please login again using the new password!");
					var update = Builders<BsonDocument>.Update.Set("onlineUsers", onlinePeeps - 1);
					collection.UpdateOne(filter, update);
					Close();
					win1.Show();
				}

				if(!approved)
                {
					MessageBox.Show("Your account has not been approved!");
					timer.Stop();
					Close();
				}

				//User gets restricted
				if (restricted)
				{
					timer.Stop();
					MessageBox.Show("Error 401 - Not Authorized", "Your access to this app has been removed.");
					Close();
				}

				onlineCount.Content = onlinePeeps.ToString();

                if (!answersDisabled)
                {
					userResponse.Content = "";
					GetAsnwersBtn.IsEnabled = true;
				} else if (answersDisabled)
                {
					userResponse.Foreground = Brushes.PaleVioletRed;
					userResponse.Content = "Cheats have been disabled by an administrator!";
					GetAsnwersBtn.IsEnabled = false;
				}

				if ((string)versionLabel.Content == version || (string)versionLabel.Content == opt_version)
				{
					return;
				}
				else
				{
					timer.Stop();
					MessageBox.Show("Error: Out Of Date", "You are using an outdated version! Please download the new version to continue using this app.");
					Close();
				}

				if (disabled)
				{
					timer.Stop();
					MessageBox.Show("Error: Disabled", "This app has been disabled by a developer!");
					Close();
				}
			});
		}
		private void ExitClicked(object sender, RoutedEventArgs e)
		{
			var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
			var database = client.GetDatabase("main");
			var collection = database.GetCollection<BsonDocument>("utils");
			var filter = Builders<BsonDocument>.Filter.Eq("data_id", 9902);
			var dbData = collection.Find(filter).FirstOrDefault();
			string data = utils.GetManifest();
			JObject jObject = JObject.Parse(data);
			int onlineUsers = (int)jObject.SelectToken("onlineUsers");
			var update = Builders<BsonDocument>.Update.Set("onlineUsers", onlineUsers - 1);
			collection.UpdateOne(filter, update);
			utils.SetStatus(username, "offline");
			timer.Stop();
			Close();
		}

		private static string GetDeviceID()
		{
			return new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().ToString();
		}

		private void ConsoleClicked(object sender, RoutedEventArgs e)
		{
			var deviceID = GetDeviceID();
			string userD = utils.GetUserData(username);
			JObject jParser = JObject.Parse(userD);
			bool isDev = (bool)jParser.SelectToken("isDev");

			if (!isDev)
			{
				MessageBox.Show("Error: Not Allowed", "This feature can only be used by the developer.");
				return;
			}
			isConsoleOpen = true;
			console.Show();
		}

		public async void SerialClicked(object sender, RoutedEventArgs e)
		{
			string data = utils.GetManifest();
			JObject jObject = JObject.Parse(data);
			bool answersDisabled = (bool)jObject.SelectToken("answersDisabled");

            if (answersDisabled)
            {
				userResponse.Foreground = Brushes.PaleVioletRed;
				userResponse.Content = "Cheats have been disabled by an administrator!";
				GetAsnwersBtn.IsEnabled = false;
				return;
			}

			string code2 = QuizIdBox.Text;
			if (int.TryParse(QuizIdBox.Text, out var _))
			{
				try
				{
					CurrentQuiz = await GetQuizInfoInfo(code2);
					setQuestionData(CurrentQuiz.data.questions);
				}
				catch (HttpRequestException)
				{
					if (KeyInfo != null)
					{
						PremiumScan(code2);
					}
					else
					{
						PremiumScan(code2);
					}
				}
				catch (ArgumentException)
				{
					if (KeyInfo != null)
					{
						PremiumScan(code2);
					}
					else
					{
						PremiumScan(code2);
					}
				}
			}
			else if (QuizIdBox.Text.Contains("/join?gc="))
			{
				string url2 = QuizIdBox.Text;
				if (url2.Contains("&"))
				{
					int start = url2.IndexOf("/join?gc=") + 9;
					int end = url2.IndexOf("&");
					code2 = url2.Substring(start, end - start);
				}
				else
				{
					int found2 = url2.IndexOf("/join?gc=") + 9;
					code2 = url2.Substring(found2, url2.Length - found2);
				}
				try
				{
					CurrentQuiz = await GetQuizInfoInfo(code2);
					setQuestionData(CurrentQuiz.data.questions);
				}
				catch (HttpRequestException)
				{
					if (KeyInfo != null)
					{
						PremiumScan(code2);
					}
					else
					{
						PremiumScan(code2);
					}
				}
				catch (ArgumentException)
				{
					if (KeyInfo != null)
					{
						PremiumScan(code2);
					}
					else
					{
						PremiumScan(code2);
					}
				}
			}
			else if (QuizIdBox.Text.Contains("quiz/") && !QuizIdBox.Text.Contains("admin") && QuizIdBox.Text.Length >= 30)
			{
				string url = QuizIdBox.Text;
				int found = url.IndexOf("quiz/");
				CurrentQuiz = await getQuizInfo("https://quizizz.com/api/main/" + url.Substring(found, 29) + "?bypassProfanity=true&returnPrivileges=true&source=join");
				try
				{
					if (CurrentQuiz.data != null)
					{
						if (KeyInfo != null)
						{
							setQuestionData(CurrentQuiz.data.quiz.info.questions);
							return;
						}
						userResponse.Foreground = Brushes.Violet;
						userResponse.Content = "This is a Premium quizz";
						GetAsnwersBtn.IsEnabled = true;
					}
					else
					{
						MessageBox.Show(this, "Not a valid quizizz url adress", "Wrong Url Adress", MessageBoxButton.OK, MessageBoxImage.Exclamation);
						GetAsnwersBtn.IsEnabled = true;
					}
				}
				catch (NullReferenceException)
				{
					MessageBox.Show(this, "Not a valid quizizz url adress", "Wrong Url Adress", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					GetAsnwersBtn.IsEnabled = true;
				}
			}
			else if (!QuizIdBox.Text.Contains("?gameType=async"))
			{
				MessageBox.Show(this, "Not a valid url. PLEASE USE THE CODE", "Wrong Url Adress", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				GetAsnwersBtn.IsEnabled = true;
			}
		}

		private async void PremiumScan(string code)
		{
			Structs.GetQuizResp VerRes = await FromVer(code);
			if (!VerRes.Success)
			{
				Structs.GetQuizResp SearchRes = await searchFor(code);
				if (!SearchRes.Success)
				{
					Structs.GetQuizResp RecoRes = await FindRecommended(code);
					if (!RecoRes.Success)
					{
						userResponse.Foreground = Brushes.PaleVioletRed;
						userResponse.Content = "Quiz not found";
						GetAsnwersBtn.IsEnabled = true;
					}
					else
					{
						setQuestionData(RecoRes.Quiz.info.questions);
					}
				}
				else
				{
					setQuestionData(SearchRes.Quiz.info.questions);
				}
			}
			else
			{
				setQuestionData(VerRes.Quiz.info.questions);
			}
		}

		private void SearchChanged(object sender, TextChangedEventArgs e)
		{
			string SearchText = SearchBox.Text;
			if (SearchText.Length > 0)
			{
				foreach (StackPanel child2 in StackPanel.Children.OfType<StackPanel>())
				{
					foreach (Label label2 in child2.Children.OfType<Label>())
					{
						string Combo = string.Empty;
						List<Label> CurrentLabels = new List<Label>();
						foreach (Label sublabel in (label2.Parent as StackPanel).Children.OfType<Label>())
						{
							Combo += sublabel.Content;
							CurrentLabels.Add(sublabel);
						}
						if (Combo.ToLower().Contains(SearchText.ToLower()))
						{
							foreach (Label current2 in CurrentLabels)
							{
								if (current2.Foreground == (Brush)FindResource("SpecialQuestion"))
								{
									current2.Foreground = (Brush)FindResource("SpecialQuestionSearch");
									current2.BringIntoView();
								}
								else if (current2.Foreground != (Brush)FindResource("AnswerColor") && current2.Foreground != (Brush)FindResource("SpecialQuestionSearch"))
								{
									current2.Foreground = (Brush)FindResource("QuestionSearch");
									current2.BringIntoView();
								}
							}
							continue;
						}
						foreach (Label current in CurrentLabels)
						{
							if (current.Foreground == (Brush)FindResource("SpecialQuestionSearch"))
							{
								current.Foreground = (Brush)FindResource("SpecialQuestion");
							}
							else if (current.Foreground != (Brush)FindResource("AnswerColor") && current.Foreground != (Brush)FindResource("SpecialQuestion"))
							{
								current.Foreground = (Brush)FindResource("FontColor");
							}
						}
					}
				}
				return;
			}
			foreach (StackPanel child in StackPanel.Children.OfType<StackPanel>())
			{
				foreach (Label label in child.Children.OfType<Label>())
				{
					if (label.Content.ToString()!.ToLower().Contains(SearchText.ToLower()))
					{
						if (label.Foreground == (Brush)FindResource("SpecialQuestionSearch"))
						{
							label.Foreground = (Brush)FindResource("SpecialQuestion");
						}
						else if (label.Foreground != (Brush)FindResource("AnswerColor") && label.Foreground != (Brush)FindResource("SpecialQuestion"))
						{
							label.Foreground = (Brush)FindResource("FontColor");
						}
					}
				}
			}
		}

		private void VersionLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://github.com/AndyFilter/QuizizzSupport/releases/latest")
			{
				UseShellExecute = true
			});
		}

		private Utils.GitHubRelease CheckVersion(string username, string repoName)
		{
			WebClient webClient = new WebClient();
			webClient.Headers.Add("User-Agent", "Unity web player");
			Uri uri = new Uri($"https://api.github.com/repos/{username}/{repoName}/releases/latest");
			string releases = webClient.DownloadString(uri);
			return JsonSerializer.Deserialize<Utils.GitHubRelease>(releases);
		}



		private async Task<Structs.GetQuizResp> FromVer(string code)
		{
			userResponse.Foreground = Brushes.WhiteSmoke;
			userResponse.Content = "Please Wait...";
			new List<string>();
			Dictionary<string, string> values = new Dictionary<string, string> { { "roomCode", code } };
			try
			{
				FormUrlEncodedContent content = new FormUrlEncodedContent(values);
				Utils.RoomData roomObject = JsonSerializer.Deserialize<Utils.RoomData>(await (await client.PostAsync("https://game.quizizz.com/play-api/v4/checkRoom", content)).Content.ReadAsStringAsync());
				if (roomObject.room != null)
				{
					string quizCode;
					try
					{
						quizCode = (int.Parse(roomObject.room.versionId.Substring(roomObject.room.versionId.Length - 2), NumberStyles.HexNumber) - 1).ToString("X").ToLower();
					}
					catch
					{
						return new Structs.GetQuizResp
						{
							Success = false
						};
					}
					string quizId = roomObject.room.versionId[0..^2] + quizCode;
					Utils.QuizData currentQuizObject = JsonSerializer.Deserialize<Utils.QuizData>(await client.GetStringAsync("https://quizizz.com/api/main/quiz/" + quizId + "?bypassProfanity=true&returnPrivileges=true&source=join"));
					return new Structs.GetQuizResp
					{
						Success = true,
						Quiz = currentQuizObject.data.quiz
					};
				}
				return new Structs.GetQuizResp
				{
					Success = false
				};
			}
			catch
			{
				return new Structs.GetQuizResp
				{
					Success = false
				};
			}
		}

		private async void GetFromIt(string code)
		{
			userResponse.Foreground = Brushes.WhiteSmoke;
			userResponse.Content = "Please Wait...";
			new List<string>();
			Dictionary<string, string> values = new Dictionary<string, string> { { "roomCode", code } };
			FormUrlEncodedContent content = new FormUrlEncodedContent(values);
			Utils.RoomData roomObject = JsonSerializer.Deserialize<Utils.RoomData>(await (await client.PostAsync("https://game.quizizz.com/play-api/v4/checkRoom", content)).Content.ReadAsStringAsync());
			values = new Dictionary<string, string>
		{
			{
				"roomHash",
				roomObject.room.hash
			},
			{
				"type",
				roomObject.room.type
			}
		};
			content = new FormUrlEncodedContent(values);
			Utils.QuestionResponse QuestionsObject = JsonSerializer.Deserialize<Utils.QuestionResponse>(await (await client.PostAsync("https://game.quizizz.com/play-api/v4/getQuestions", content)).Content.ReadAsStringAsync());
			foreach (KeyValuePair<string, Utils.Questions> question in QuestionsObject.questions)
			{
				_ = question;
			}
		}

		private void SetSingleQuestion(Utils.Questions question, Utils.Answer AnswerObject)
		{
			if (KeyInfo.QuizzesScanned + 1 > KeyInfo.MaxQuizzes)
			{
				userResponse.Foreground = Brushes.PaleVioletRed;
				userResponse.Content = "Quiz limit reached";
				return;
			}
			userResponse.Foreground = Brushes.LimeGreen;
			userResponse.Content = "Quiz Found!";
			Label Question = new Label();
			Label Answer = new Label();
			StackPanel QuestData = new StackPanel();
			QuestData.Orientation = Orientation.Horizontal;
			QuestData.Children.Add(Question);
			QuestData.Children.Add(Answer);
			string answer = "";
			switch (question.type)
			{
				case "quiz":
				case "BLANK":
					Question.Content = question.structure.query.text;
					break;
				case "MSQ":
				case "MCQ":
					Question.Content = question.structure.query.text;
					Answer.Content = AnswerObject.answer;
					break;
			}
			string _question = Utils.RemoveHtml(question.structure.query.text);
			if (question.structure.query.media.Count > 0)
			{
				Image td = new Image();
				td.MaxHeight = 400.0;
				td.MaxWidth = 400.0;
				td.Source = new BitmapImage(new Uri(question.structure.query.media[0].url));
				ToolTipService.SetToolTip((DependencyObject)(object)Question, td);
				if (_question.Length < 2)
				{
					_question = "(image)";
				}
			}
			Question.Content = _question + "    -";
			Answer.Content = answer;
			Answer.HorizontalAlignment = HorizontalAlignment.Left;
			Question.HorizontalAlignment = HorizontalAlignment.Left;
			StackPanel.Children.Add(QuestData);
		}

		private async Task<Structs.GetQuizResp> FindRecommended(string code)
		{
			userResponse.Foreground = Brushes.WhiteSmoke;
			userResponse.Content = "Please Wait...";
			List<string> ScannedQuizes = new List<string>();
			new List<string>();
			Dictionary<string, string> values = new Dictionary<string, string> { { "roomCode", code } };
			FormUrlEncodedContent content = new FormUrlEncodedContent(values);
			Utils.RoomData roomObject = JsonSerializer.Deserialize<Utils.RoomData>(await (await client.PostAsync("https://game.quizizz.com/play-api/v4/checkRoom", content)).Content.ReadAsStringAsync());
			if (roomObject.room != null)
			{
				List<string> quizTag = roomObject.room.questions;
				string quizLongId = roomObject.room.quizId;
				Utils.GameSummary SummaryObject = JsonSerializer.Deserialize<Utils.GameSummary>(await client.GetStringAsync("http://quizizz.com/api/main/gameSummaryRec?quizId=" + quizLongId));
				foreach (Utils.SummaryQuiz RecomendedQuiz in SummaryObject.data.quizzes)
				{
					if (ScannedQuizes.Contains(RecomendedQuiz._id))
					{
						continue;
					}
					Debug.Content = RecomendedQuiz.info.name;
					string RecomendedQuizString;
					try
					{
						RecomendedQuizString = await client.GetStringAsync("https://quizizz.com/api/main/gameSummaryRec?quizId=" + RecomendedQuiz._id);
					}
					catch
					{
						continue;
					}
					Utils.GameSummary RecomendedQuizObject = JsonSerializer.Deserialize<Utils.GameSummary>(RecomendedQuizString);
					ScannedQuizes.Add(RecomendedQuiz._id);
					foreach (Utils.SummaryQuiz quiz in RecomendedQuizObject.data.quizzes)
					{
						Debug.Content = quiz.info.name;
						string RecomendedQuizString2;
						try
						{
							RecomendedQuizString2 = await client.GetStringAsync("https://quizizz.com/api/main/gameSummaryRec?quizId=" + quiz._id);
						}
						catch
						{
							continue;
						}
						Utils.GameSummary RecomendedQuizObject2 = JsonSerializer.Deserialize<Utils.GameSummary>(RecomendedQuizString2);
						foreach (Utils.SummaryQuiz quiz2 in RecomendedQuizObject2.data.quizzes)
						{
							if (!ScannedQuizes.Contains(quiz2._id))
							{
								Debug.Content = quiz2.info.name;
								string currentQuiz;
								try
								{
									currentQuiz = await client.GetStringAsync("https://quizizz.com/quiz/" + quiz2._id);
								}
								catch
								{
									continue;
								}
								Utils.QuizData currentQuizObject = JsonSerializer.Deserialize<Utils.QuizData>(currentQuiz);
								ScannedQuizes.Add(quiz2._id);
								if (quizTag[0] == currentQuizObject.data.quiz.info.questions[0]._id && quizTag[1] == currentQuizObject.data.quiz.info.questions[1]._id)
								{
									return new Structs.GetQuizResp
									{
										Success = true,
										Quiz = currentQuizObject.data.quiz
									};
								}
							}
						}
					}
				}
				userResponse.Foreground = Brushes.PaleVioletRed;
				userResponse.Content = "Quiz not found";
				return new Structs.GetQuizResp
				{
					Success = false
				};
			}
			userResponse.Foreground = Brushes.PaleVioletRed;
			userResponse.Content = "Quiz not found";
			return new Structs.GetQuizResp
			{
				Success = false
			};
		}

		private async Task<Structs.GetQuizResp> searchFor(string code)
		{
			userResponse.Foreground = Brushes.WhiteSmoke;
			userResponse.Content = "Please Wait...";
			Dictionary<string, string> values = new Dictionary<string, string> { { "roomCode", code } };
			FormUrlEncodedContent content = new FormUrlEncodedContent(values);
			Utils.RoomData roomObject = JsonSerializer.Deserialize<Utils.RoomData>(await (await client.PostAsync("https://game.quizizz.com/play-api/v4/checkRoom", content)).Content.ReadAsStringAsync());
			if (roomObject.room != null)
			{
				string quizTag = roomObject.room.questions[0];
				int numberOfQuestions = roomObject.room.questions.Count;
				string quizLongId = roomObject.room.quizId;
				Utils.InfoData SummaryObject = JsonSerializer.Deserialize<Utils.InfoData>(await client.GetStringAsync("https://quizizz.com/api/main/quiz/" + quizLongId + "/info"));
				Utils.SearchArgs searchArgs = new Utils.SearchArgs();
				Utils.QuestionsRange RangeList = new Utils.QuestionsRange();
				searchArgs.occupation = new List<string> { "teacher", "teacher_school", "teacher_university", "other", "student" };
				searchArgs.SubjectsAggs = SummaryObject.subjects;
				searchArgs.grade = SummaryObject.grade;
				RangeList.numberOfQuestions = new List<int> { numberOfQuestions, numberOfQuestions };
				Regex rgx = new Regex("[^\\p{L}\\p{N}]+");
				string str = (SummaryObject.name = rgx.Replace(SummaryObject.name, " "));
				string searchName = HttpUtility.UrlEncode(str);
				Utils.MultiQuizz searchObject = JsonSerializer.Deserialize<Utils.MultiQuizz>(await client.GetStringAsync("https://quizizz.com/api/main/search?sortKey=_score&filterList=" + JsonSerializer.Serialize(searchArgs) + "&rangeList=" + JsonSerializer.Serialize(RangeList) + "&page=SearchPage&from=0&query=" + searchName + "&size=40"));
				foreach (Utils.Quiz quiz2 in searchObject.data.hits)
				{
					if (quiz2.info.questions[0]._id == quizTag)
					{
						return new Structs.GetQuizResp
						{
							Success = true,
							Quiz = quiz2
						};
					}
				}
				Utils.GameSummary RecommendedQuizzesObject = JsonSerializer.Deserialize<Utils.GameSummary>(await client.GetStringAsync("https://quizizz.com/api/main/gameSummaryRec?quizId=" + quizLongId));
				Utils.QuizData RecommendedQuiz = JsonSerializer.Deserialize<Utils.QuizData>(await client.GetStringAsync("https://quizizz.com/quiz/" + RecommendedQuizzesObject.data.quizzes[0]._id));
				searchArgs.LangAggs = new List<string> { RecommendedQuiz.data.quiz.info.lang };
				searchObject = JsonSerializer.Deserialize<Utils.MultiQuizz>(await client.GetStringAsync("https://quizizz.com/api/main/search?sortKey=_score&filterList=" + JsonSerializer.Serialize(searchArgs) + "&rangeList=" + JsonSerializer.Serialize(RangeList) + "&page=SearchPage&from=0&query=" + searchName + "&size=40"));
				foreach (Utils.Quiz quiz in searchObject.data.hits)
				{
					if (quiz.info.questions[0]._id == quizTag)
					{
						return new Structs.GetQuizResp
						{
							Success = true,
							Quiz = quiz
						};
					}
				}
				return new Structs.GetQuizResp
				{
					Success = false
				};
			}
			userResponse.Foreground = Brushes.PaleVioletRed;
			userResponse.Content = "Quiz not found";
			return new Structs.GetQuizResp
			{
				Success = false
			};
		}

		private async Task<Utils.QuizData> getQuizInfo(string url)
		{
			string quizinfo;
			try
			{
				quizinfo = await client.GetStringAsync(url);
			}
			catch (HttpRequestException)
			{
				return new Utils.QuizData();
			}
			catch (ArgumentNullException)
			{
				return new Utils.QuizData();
			}
			catch (ArgumentOutOfRangeException)
			{
				return new Utils.QuizData();
			}
			return JsonSerializer.Deserialize<Utils.QuizData>(quizinfo);
		}

		private async Task<Utils.QuizData> GetQuizInfoInfo(string code)
		{
			Dictionary<string, string> values = new Dictionary<string, string> {
		{
			"roomCode",
			code.ToString()
		} };
			FormUrlEncodedContent content = new FormUrlEncodedContent(values);
			string codeString = await (await client.PostAsync("https://game.quizizz.com/play-api/v4/checkRoom", content)).Content.ReadAsStringAsync();
			if (JsonSerializer.Deserialize<Utils.RoomData>(codeString).room == null)
			{
				throw new ArgumentException("Parameter cannot be null", "getQuizInfo");
			}
			return JsonSerializer.Deserialize<Utils.QuizData>(await client.GetStringAsync("https://quizizz.com/api/main/game/" + JsonSerializer.Deserialize<Utils.RoomData>(codeString).room.hash));
		}

		private async void setQuestionData(List<Utils.Questions> questions)
		{
			if (KeyInfo != null && KeyInfo.QuizzesScanned + 1 > KeyInfo.MaxQuizzes)
			{
				userResponse.Foreground = Brushes.PaleVioletRed;
				userResponse.Content = "Quiz limit reached";
				GetAsnwersBtn.IsEnabled = true;
				return;
			}
			userResponse.Foreground = Brushes.LimeGreen;
			userResponse.Content = "Quiz Found!";
			GetAsnwersBtn.IsEnabled = true;
			StackPanel.Children.Clear();
			foreach (Utils.Questions question in questions)
			{
				if (!question.structure.settings.hasCorrectAnswer)
				{
					continue;
				}
				Label Question = new Label();
				Label Answer = new Label();
				StackPanel QuestData = new StackPanel();
				QuestData.Orientation = Orientation.Horizontal;
				QuestData.Children.Add(Question);
				QuestData.Children.Add(Answer);
				Answer.Foreground = (Brush)FindResource("AnswerColor");
				string answer = "";
				TexFormulaParser parser = new TexFormulaParser();
				switch (question.type)
				{
					case "quiz":
					case "BLANK":
						_ = (JsonElement)question.structure.answer;
						answer += Utils.RemoveHtml(question.structure.options[0].text);
						if (question.structure.options[0].media.Count > 0)
						{
							Image td6 = new Image();
							td6.MaxHeight = 400.0;
							td6.Source = new BitmapImage(new Uri(question.structure.options[0].media[0].url));
							ToolTipService.SetToolTip((DependencyObject)(object)Answer, td6);
							if (answer.Length < 1)
							{
								answer = "(Image)";
							}
						}
						else
						{
							if (!question.structure.options[0].hasMath)
							{
								break;
							}
							foreach (string latex2 in question.structure.options[0].math.latex)
							{
								try
								{
									TexFormula formula4 = parser.Parse(latex2);
									byte[] pngBytes4 = formula4.RenderToPng(200.0, 0.0, 0.0, "Arial");
									Label image2 = new Label();
									Image td5 = new Image();
									image2.Foreground = (Brush)FindResource("AnswerColor");
									td5.MaxHeight = 400.0;
									td5.MaxWidth = 400.0;
									td5.Source = LoadImage(pngBytes4);
									ToolTipService.SetToolTip((DependencyObject)(object)image2, td5);
									QuestData.Children.Add(image2);
									image2.Content = "(Math)";
									if (question.structure.options[0].math.latex.Count - question.structure.options[0].math.latex.IndexOf(latex2) > 1)
									{
										image2.Content = image2.Content?.ToString() + "  &  ";
									}
								}
								catch
								{
									TexFormula formula3 = parser.Parse(Utils.CleanLatex(latex2));
									byte[] pngBytes3 = formula3.RenderToPng(200.0, 0.0, 0.0, "Arial");
									Label image = new Label();
									Image td4 = new Image();
									image.Foreground = (Brush)FindResource("AnswerColor");
									td4.MaxHeight = 400.0;
									td4.MaxWidth = 400.0;
									td4.Source = LoadImage(pngBytes3);
									ToolTipService.SetToolTip((DependencyObject)(object)image, td4);
									QuestData.Children.Add(image);
									image.Content = "(Math)";
									if (question.structure.options[0].math.latex.Count - question.structure.options[0].math.latex.IndexOf(latex2) > 1)
									{
										image.Content = image.Content?.ToString() + "  &  ";
									}
								}
							}
							if (question.structure.options[0].math.latex.Count <= 0)
							{
								answer += Utils.RemoveHtml(question.structure.options[0].text);
							}
						}
						break;
					default:
						{
							List<int> listansw = (from m in Regex.Matches(((JsonElement)question.structure.answer).GetRawText(), "(-?[0-9]+)").OfType<Match>()
												  select int.Parse(m.Value)).ToList();
							listansw.Select((int i) => i.ToString(CultureInfo.InvariantCulture)).Aggregate((string s1, string s2) => s1 + ", " + s2);
							if (listansw.Count <= 0)
							{
								answer += Utils.RemoveHtml(question.structure.options[0].text);
								if (question.structure.options[0].media.Count > 0)
								{
									Image td8 = new Image();
									td8.MaxHeight = 400.0;
									td8.MaxWidth = 400.0;
									td8.Source = new BitmapImage(new Uri(question.structure.options[0].media[0].url));
									ToolTipService.SetToolTip((DependencyObject)(object)Answer, td8);
									if (answer.Length < 1)
									{
										answer = "(Image)";
									}
								}
								else if (question.structure.options[0].hasMath)
								{
									foreach (string latex3 in question.structure.options[0].math.latex)
									{
										try
										{
											TexFormula formula7 = parser.Parse(latex3);
											byte[] pngBytes7 = formula7.RenderToPng(200.0, 0.0, 0.0, "Arial");
											Label image5 = new Label();
											image5.Foreground = (Brush)FindResource("AnswerColor");
											Image td11 = new Image();
											td11.MaxHeight = 400.0;
											td11.MaxWidth = 400.0;
											td11.Source = LoadImage(pngBytes7);
											ToolTipService.SetToolTip((DependencyObject)(object)image5, td11);
											QuestData.Children.Add(image5);
											image5.Content = "(Math)";
											if (question.structure.options[0].math.latex.Count - question.structure.options[0].math.latex.IndexOf(latex3) > 1)
											{
												image5.Content = image5.Content?.ToString() + "  &  ";
											}
										}
										catch
										{
											TexFormula formula8 = parser.Parse(Utils.CleanLatex(latex3));
											byte[] pngBytes8 = formula8.RenderToPng(200.0, 0.0, 0.0, "Arial");
											Label image7 = new Label();
											Image td12 = new Image();
											image7.Foreground = (Brush)FindResource("AnswerColor");
											td12.MaxHeight = 400.0;
											td12.MaxWidth = 400.0;
											td12.Source = LoadImage(pngBytes8);
											ToolTipService.SetToolTip((DependencyObject)(object)image7, td12);
											QuestData.Children.Add(image7);
											image7.Content = "(Math)";
											if (question.structure.options[0].math.latex.Count - question.structure.options[0].math.latex.IndexOf(latex3) > 1)
											{
												image7.Content = image7.Content?.ToString() + "  &  ";
											}
										}
										if (question.structure.options[0].math.latex.Count <= 0)
										{
											answer += Utils.RemoveHtml(question.structure.options[0].text);
										}
									}
								}
							}
							foreach (int j in listansw)
							{
								if (question.structure.options[j].media.Count > 0)
								{
									foreach (Utils.Media media in question.structure.options[j].media)
									{
										Label image6 = new Label();
										Image td10 = new Image();
										image6.Foreground = (Brush)FindResource("AnswerColor");
										td10.MaxHeight = 400.0;
										td10.MaxWidth = 400.0;
										td10.Source = new BitmapImage(new Uri(media.url));
										ToolTipService.SetToolTip((DependencyObject)(object)image6, td10);
										QuestData.Children.Add(image6);
										image6.Content = "(Image)";
										if (listansw.Count - listansw.IndexOf(j) > 1)
										{
											image6.Content = image6.Content?.ToString() + "  &  ";
										}
									}
								}
								else if (question.structure.options[j].hasMath)
								{
									foreach (string latex4 in question.structure.options[j].math.latex)
									{
										try
										{
											TexFormula formula6 = parser.Parse(latex4);
											byte[] pngBytes6 = formula6.RenderToPng(200.0, 0.0, 0.0, "Arial");
											Label image4 = new Label();
											Image td9 = new Image();
											image4.Foreground = (Brush)FindResource("AnswerColor");
											td9.MaxHeight = 400.0;
											td9.MaxWidth = 400.0;
											td9.Source = LoadImage(pngBytes6);
											ToolTipService.SetToolTip((DependencyObject)(object)image4, td9);
											QuestData.Children.Add(image4);
											image4.Content = "(Math)";
											if (listansw.Count - listansw.IndexOf(j) > 1)
											{
												image4.Content = image4.Content?.ToString() + "  &  ";
											}
										}
										catch
										{
											Utils.CleanLatex(latex4);
											TexFormula formula5 = parser.Parse(Utils.CleanLatex(latex4));
											byte[] pngBytes5 = formula5.RenderToPng(200.0, 0.0, 0.0, "Arial");
											Label image3 = new Label();
											Image td7 = new Image();
											image3.Foreground = (Brush)FindResource("AnswerColor");
											td7.MaxHeight = 400.0;
											td7.MaxWidth = 400.0;
											td7.Source = LoadImage(pngBytes5);
											ToolTipService.SetToolTip((DependencyObject)(object)image3, td7);
											QuestData.Children.Add(image3);
											image3.Content = "(Math)";
											if (listansw.Count - listansw.IndexOf(j) > 1)
											{
												image3.Content = image3.Content?.ToString() + "  &  ";
											}
										}
									}
									if (question.structure.options[j].math.latex.Count <= 0)
									{
										answer += Utils.RemoveHtml(question.structure.options[j].text);
									}
								}
								else
								{
									if (j >= 0)
									{
										answer += Utils.RemoveHtml(question.structure.options[j].text);
									}
									if (listansw.Count - listansw.IndexOf(j) > 1)
									{
										answer += " & ";
									}
								}
							}
							break;
						}
					case "SLIDE":
						continue;
				}
				string _question = Utils.RemoveHtml(question.structure.query.text);
				if (question.structure.query.media.Count > 0)
				{
					Image td3 = new Image();
					td3.MaxHeight = 400.0;
					td3.MaxWidth = 400.0;
					Question.Foreground = (Brush)FindResource("SpecialQuestion");
					td3.Source = new BitmapImage(new Uri(question.structure.query.media[0].url));
					ToolTipService.SetToolTip((DependencyObject)(object)Question, td3);
					if (_question.Length < 2)
					{
						_question = "(Image)";
					}
				}
				else if (question.structure.query.hasMath)
				{
					foreach (string latex in question.structure.query.math.latex)
					{
						try
						{
							TexFormula formula2 = parser.Parse(latex);
							byte[] pngBytes2 = formula2.RenderToPng(200.0, 0.0, 0.0, "Arial");
							Image td2 = new Image();
							Question.Foreground = (Brush)FindResource("SpecialQuestion");
							td2.MaxHeight = 400.0;
							td2.MaxWidth = 400.0;
							td2.Source = LoadImage(pngBytes2);
							ToolTipService.SetToolTip((DependencyObject)(object)Question, td2);
						}
						catch
						{
							TexFormula formula = parser.Parse(Utils.CleanLatex(latex));
							byte[] pngBytes = formula.RenderToPng(200.0, 0.0, 0.0, "Arial");
							Image td = new Image();
							Question.Foreground = (Brush)FindResource("SpecialQuestion");
							td.MaxHeight = 400.0;
							td.MaxWidth = 400.0;
							td.Source = LoadImage(pngBytes);
							ToolTipService.SetToolTip((DependencyObject)(object)Question, td);
						}
					}
					string cleanQuestion = Utils.RemoveHtml(question.structure.query.math.template);
					_question = cleanQuestion.Substring(0, cleanQuestion.Length - 4);
					if (_question.Length < 2)
					{
						_question = "(Math)";
					}
				}
				Question.Content = _question + "   -  ";
				Answer.Content = answer;
				Answer.HorizontalAlignment = HorizontalAlignment.Left;
				Question.HorizontalAlignment = HorizontalAlignment.Left;
				StackPanel.Children.Add(QuestData);
			}
			if (KeyInfo != null)
			{
				KeyInfo = await Utils.AddQuizzesToData(KeyInfo.Key, 1);
			}
		}

		private static BitmapImage LoadImage(byte[] imageData)
		{
			if (imageData == null || imageData.Length == 0)
			{
				return null;
			}
			BitmapImage image = new BitmapImage();
			using (MemoryStream mem = new MemoryStream(imageData))
			{
				mem.Position = 0L;
				image.BeginInit();
				image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = null;
				image.StreamSource = mem;
				image.EndInit();
			}
			((Freezable)image).Freeze();
			return image;
		}

		private void EnterPressed(object sender, KeyEventArgs e)
		{
			if ((int)e.Key == 6)
			{
				SerialClicked(sender, e);
			}
		}

		private void MouseTabDrag(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void CopyClicked(object sender, RoutedEventArgs e)
		{
			string AllData = "";
			foreach (StackPanel child in StackPanel.Children.OfType<StackPanel>())
			{
				foreach (Label label in child.Children.OfType<Label>())
				{
					if (child.Children.IndexOf(label) == 0)
					{
						AllData += $" {label.Content}    {(child.Children[child.Children.IndexOf(label) + 1] as Label).Content}\n";
					}
				}
			}
			Clipboard.SetText(AllData);
		}
		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
		}

		private void MinimalizeClicked(object sender, RoutedEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		private void MaximizeClicked(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			switch (base.WindowState)
			{
				case WindowState.Maximized:
					base.WindowState = WindowState.Normal;
					button.Content = "◻";
					break;
				case WindowState.Normal:
					base.WindowState = WindowState.Maximized;
					button.Content = "❏";
					break;
			}
		}
		private void DiscordClicked(object sender, MouseButtonEventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://discord.gg/q5jzED6Tnd")
			{
				UseShellExecute = true
			});
		}

        private void PinClicked(object sender, RoutedEventArgs e)
        {
			Window2 helpWin = new Window2();
			helpWin.Show();
		}

        private void PayPalClicked(object sender, MouseButtonEventArgs e)
        {
			Process.Start(new ProcessStartInfo("https://paypal.me/xbl4z3r")
			{
				UseShellExecute = true
			});
		}

		private void EditProfile(object sender, RoutedEventArgs e)
        {
			EditProfile profileTab = new EditProfile(username);
			profileTab.Show();
        }
    }
}

