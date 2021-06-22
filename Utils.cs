// QuizizzSupport.Utils
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeviceId;
using Google.Cloud.Firestore;
using HyperHack;
using MongoDB.Bson;
using MongoDB.Driver;

internal class Utils
{
	public class QuizData
	{
		public Data data { get; set; }

		public bool success { get; set; }
	}

	public class Data
	{
		public Quiz quiz { get; set; }

		public List<Questions> questions { get; set; }
	}

	public class Quiz
	{
		public Info info { get; set; }
	}

	public class Info
	{
		public List<Questions> questions { get; set; }

		public string lang { get; set; }
	}

	public class Questions
	{
		public Structure structure { get; set; }

		public string type { get; set; }

		public string _id { get; set; }
	}

	public class Structure
	{
		public Settings settings { get; set; }

		public Query query { get; set; }

		public List<Options> options { get; set; }

		public dynamic answer { get; set; }
	}

	public class Settings
	{
		public bool hasCorrectAnswer { get; set; }
	}

	public class Query
	{
		public string text { get; set; }

		public List<Media> media { get; set; }

		public bool hasMath { get; set; }

		public Math math { get; set; }
	}

	public class Math
	{
		public List<string> latex { get; set; }

		public string template { get; set; }
	}

	public class Media
	{
		public string type { get; set; }

		public string url { get; set; }
	}

	public class Options
	{
		public List<Answers> options { get; set; }

		public string text { get; set; }

		public List<Media> media { get; set; }

		public bool hasMath { get; set; }

		public Math math { get; set; }
	}

	public class Answers
	{
		public string text { get; set; }
	}

	public class RoomData
	{
		public Room room { get; set; }
	}

	public class Room
	{
		public string hash { get; set; }

		public string quizId { get; set; }

		public string versionId { get; set; }

		public List<string> questions { get; set; }

		public string type { get; set; }
	}

	public class Version
	{
		public int version { get; set; }

		public string type { get; set; }
	}

	public class GameSummary
	{
		public SummaryData data { get; set; }
	}

	public class SummaryData
	{
		public List<SummaryQuiz> quizzes { get; set; }
	}

	public class SummaryQuiz
	{
		public string _id { get; set; }

		public SummaryInfo info { get; set; }
	}

	public class SummaryInfo
	{
		public string name { get; set; }
	}

	public class QuestionResponse
	{
		public Dictionary<string, Questions> questions { get; set; }
	}

	public class Answer
	{
		public string answer { get; set; }

		public string _id { get; set; }
	}

	public class InfoData
	{
		public string name { get; set; }

		public List<string> subjects { get; set; }

		public List<string> grade { get; set; }
	}

	public class SearchArgs
	{
		[JsonPropertyName("grade_type.aggs")]
		public List<object> GradeTypeAggs { get; set; }

		public List<string> occupation { get; set; }

		public List<bool> cloned { get; set; }

		[JsonPropertyName("subjects.aggs")]
		public List<string> SubjectsAggs { get; set; }

		[JsonPropertyName("lang.aggs")]
		public List<string> LangAggs { get; set; }

		public List<string> grade { get; set; }

		public List<string> isProfane { get; set; }
	}

	public class MultiQuizz
	{
		public MultiData data { get; set; }
	}

	public class MultiData
	{
		public List<Quiz> hits { get; set; }
	}

	public class QuestionsRange
	{
		public List<int> numberOfQuestions { get; set; }
	}

	public class GitHubRelease
	{
		public string tag_name { get; set; }
	}

	public class MaxValues
	{
		public List<int> Devices = new List<int> { 0, 100 };

		public List<int> Quizzes = new List<int> { 0, 200 };

		public List<int> Days = new List<int> { 0, 100 };
	}

	public class KeyData
	{
		public bool success { get; set; }

		public int num1 { get; set; }

		public int num2 { get; set; }

		public int num3 { get; set; }
	}

	private static FirestoreDb db;

	private static readonly HttpClient client = new HttpClient();

	public static string version = "5.0";

	public static string SettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GamerMoment");

	public static string FileName = "TorojanX86.Jquery";

	public string GetManifest()
    {
		var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
		var database = client.GetDatabase("main");
		var collection = database.GetCollection<BsonDocument>("utils");
		var filter = Builders<BsonDocument>.Filter.Eq("data_id", 9902);
		var dbData = collection.Find(filter).FirstOrDefault();
		var data = dbData.ToString();
		data = data.Replace("_", "");
		data = data.Remove(9, 36);
		return data;
	}

	public string GetUserData(string username)
	{
		var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
		var database = client.GetDatabase("main");
		var collection = database.GetCollection<BsonDocument>("users");
		var filter = Builders<BsonDocument>.Filter.Eq("username", username);
		var dbData = collection.Find(filter).FirstOrDefault();
		var data = dbData.ToString();
		data = data.Replace("_", "");
		data = data.Remove(9, 36);
		return data;
	}

	public void SetStatus(string username, string option)
	{
		var client = new MongoClient("mongodb+srv://xblazer:christian06@datas.q6bjt.mongodb.net/main?retryWrites=true&w=majority");
		var database = client.GetDatabase("main");
		var collection = database.GetCollection<BsonDocument>("users");
		var filter = Builders<BsonDocument>.Filter.Eq("username", username);
		var dbData = collection.Find(filter).FirstOrDefault();
		var data = dbData.ToString();
		if(option == "online")
        {
			var update1 = Builders<BsonDocument>.Update.Set("isOnline", true);
			collection.UpdateOne(filter, update1);
		} else if(option == "offline")
        {
			var update2 = Builders<BsonDocument>.Update.Set("isOnline", false);
			collection.UpdateOne(filter, update2);
		}

	}

	public static string RemoveHtml(string htmlstring)
	{
		string clean = Regex.Replace(htmlstring, "<.*?>", string.Empty);
		return Regex.Replace(clean, "&nbsp;", string.Empty);
	}

	public static void DeleteFilesInDir(DirectoryInfo dir, List<string> FileQuery)
	{
		FileInfo[] Files = dir.GetFiles();
		FileInfo[] array = Files;
		foreach (FileInfo file in array)
		{
			foreach (string FileName in FileQuery)
			{
				if (file.ToString().Contains(FileName) && !file.ToString().Contains("runtimeconfig") && !file.ToString().Contains("deps") && !file.ToString().Contains(".dll"))
				{
					try
					{
						file.Delete();
					}
					catch
					{
					}
				}
			}
		}
		DirectoryInfo[] Dirs = dir.GetDirectories();
		DirectoryInfo[] array2 = Dirs;
		foreach (DirectoryInfo directory in array2)
		{
			foreach (string FileName2 in FileQuery)
			{
				if (directory.Name.Contains(FileName2))
				{
					try
					{
						Directory.Delete(directory.ToString(), recursive: true);
					}
					catch
					{
					}
				}
			}
		}
	}

	public static Structs.KeyValidation CheckKey(string Key)
	{
		Structs.KeyValidation KeyStruct = new Structs.KeyValidation();
		int num4 = int.Parse(Key.Substring(0, 3), NumberStyles.HexNumber);
		int num2 = int.Parse(Key.Substring(3, 4), NumberStyles.HexNumber);
		int keyType = int.Parse(Key.Substring(7, 1), NumberStyles.HexNumber);
		int num3 = int.Parse(Key.Substring(8, 3), NumberStyles.HexNumber);
		int num1 = int.Parse(Key.Substring(11, 3), NumberStyles.HexNumber);
		int num5 = int.Parse(Key.Substring(14, 3), NumberStyles.HexNumber);
		int num6 = int.Parse(Key.Substring(17), NumberStyles.HexNumber);
		int value = int.Parse(System.Math.Cos((double)num2 / (double)num1 * (System.Math.PI / 180.0)).ToString().Substring(4, 4));
		if (value <= 0)
		{
			value = int.Parse(System.Math.Sin((double)num2 / (double)num1 * (System.Math.PI / 180.0)).ToString().Substring(4, 4));
		}
		int devi = (int)System.Math.Ceiling((double)(num3 - 252) * 2.0 / System.Math.Sqrt((double)num2 / 8.0));
		int quizes = 100;
		int days = (int)System.Math.Ceiling((double)(num5 - 256));
		string number = num1.ToString() + num2;
		List<int> numbers = new List<int>();
		int Type = keyType;
		string text = number;
		foreach (char j in text)
		{
			int _type = keyType;
			if (int.TryParse(j.ToString(), out var i) && i - _type <= 7)
			{
				numbers.Add(i);
			}
		}
		Type -= numbers.Max();
		if (num1 % 3 == 0 && num2 % 7 == 0 && value == num6 && days >= 0)
		{
			return new Structs.KeyValidation
			{
				success = true,
				Devices = devi,
				Days = days,
				Quizzes = quizes,
				keyType = (Structs.KeyType)Type
			};
		}
		return new Structs.KeyValidation
		{
			success = false,
			Devices = 0,
			Days = 0,
			Quizzes = 0,
			keyType = Structs.KeyType.none
		};
	}

	public static Structs.KeyType GetKeyType(int Devices, int Quizzes, int DaysAfterActivation)
	{
		if (!int.TryParse(Devices.ToString(), out Devices) || Devices <= 0)
		{
			Devices = 0;
		}
		if (!int.TryParse(Quizzes.ToString(), out Quizzes) || Quizzes <= 0)
		{
			Quizzes = 0;
		}
		if (!int.TryParse(DaysAfterActivation.ToString(), out DaysAfterActivation) || DaysAfterActivation <= 0)
		{
			DaysAfterActivation = 0;
		}
		if (Devices == 0 && Quizzes == 0 && DaysAfterActivation == 0)
		{
			return Structs.KeyType.none;
		}
		if (Quizzes == 0 && DaysAfterActivation == 0)
		{
			return Structs.KeyType.Dev;
		}
		if (Devices == 0 && DaysAfterActivation == 0)
		{
			return Structs.KeyType.Qui;
		}
		if (Devices == 0 && Quizzes == 0)
		{
			return Structs.KeyType.Day;
		}
		if (DaysAfterActivation == 0)
		{
			return Structs.KeyType.DevQui;
		}
		if (Quizzes == 0)
		{
			return Structs.KeyType.DevDay;
		}
		if (Devices == 0)
		{
			return Structs.KeyType.QuiDay;
		}
		return Structs.KeyType.DevQuizDay;
	}

	public static int Subint(int number, int index, int lenght)
	{
		string Str = number.ToString();
		return int.Parse(Str.Substring(index, lenght));
	}

	public static string GetDeviceID()
	{
		return new DeviceIdBuilder().AddProcessorId().AddMotherboardSerialNumber().ToString();
	}

	public static async Task<Structs.KeyInformation> AddQuizzesToData(string Key, int QuizzesToAdd)
	{
		string creds = await client.GetStringAsync(Encoding.UTF8.GetString(Convert.FromBase64String("aHR0cHM6Ly9kcml2ZS5nb29nbGUuY29tL3VjP2V4cG9ydD1kb3dubG9hZCZpZD0xSVEta3E5dkIwX1BVblBiblNWZk80aDFPT1V4VFlKcGE=")));
		db = new FirestoreDbBuilder
		{
			ProjectId = "quizizzsupport",
			JsonCredentials = creds
		}.Build();
		if (!Directory.Exists(SettingsDir))
		{
			Directory.CreateDirectory(SettingsDir);
		}
		if (File.Exists(Path.Combine(SettingsDir, FileName)))
		{
			Structs.UserInfo UserData = JsonSerializer.Deserialize<Structs.UserInfo>(File.ReadAllText(Path.Combine(SettingsDir, FileName)));
			foreach (Structs.KeyInformation key in UserData.Keys)
			{
				if (key.Key == Key)
				{
					key.QuizzesScanned += QuizzesToAdd;
					DocumentReference docRef = db.Collection("Auth").Document(Key);
					Dictionary<string, object> documentDictionary = (await docRef.GetSnapshotAsync()).ToDictionary();
					List<object> KeyList = documentDictionary["HWID"] as List<object>;
					long QuizzesScanned = (long)documentDictionary["QuizzesScanned"] + QuizzesToAdd;
					key.QuizzesScanned = (int)QuizzesScanned;
					Dictionary<string, object> user = new Dictionary<string, object>
					{
						{ "HWID", KeyList },
						{ "QuizzesScanned", QuizzesScanned }
					};
					await docRef.SetAsync(user);
					File.WriteAllText(Path.Combine(SettingsDir, FileName), JsonSerializer.Serialize(UserData));
					return key;
				}
			}
		}
		return null;
	}

	public static string CleanLatex(string Latex)
	{
		List<char> LatexArray = Latex.ToList();
		string clean = "";
		return Latex.Replace("\\\\", "\\").Replace("\\ ", "%^&").Replace("%^& %^&", " \\ ")
			.Replace("%^&", "")
			.Replace("\\ ", "\\,");
	}

	public static GitHubRelease CheckVersion(string username, string repoName)
	{
		WebClient webClient = new WebClient();
		webClient.Headers.Add("User-Agent", "Unity web player");
		Uri uri = new Uri($"https://api.github.com/repos/{username}/{repoName}/releases/latest");
		string releases = webClient.DownloadString(uri);
		return JsonSerializer.Deserialize<GitHubRelease>(releases);
	}

	public static int GetFreeScans()
	{
		if (!Directory.Exists(SettingsDir))
		{
			Directory.CreateDirectory(SettingsDir);
		}
		if (File.Exists(Path.Combine(SettingsDir, FileName)))
		{
			Structs.UserInfo UserData2 = JsonSerializer.Deserialize<Structs.UserInfo>(File.ReadAllText(Path.Combine(SettingsDir, FileName)));
			if (UserData2.Today != null)
			{
				if (UserData2.Today.Day == DateTime.Today.Date)
				{
					return UserData2.Today.Scans;
				}
				UserData2.Today = new Structs.DailyScans
				{
					Day = DateTime.Today.Date,
					Scans = 0
				};
				File.WriteAllText(Path.Combine(SettingsDir, FileName), JsonSerializer.Serialize(UserData2));
				return 0;
			}
			UserData2.Today = new Structs.DailyScans
			{
				Day = DateTime.Today.Date,
				Scans = 0
			};
			File.WriteAllText(Path.Combine(SettingsDir, FileName), JsonSerializer.Serialize(UserData2));
			return 0;
		}
		Structs.UserInfo UserData = new Structs.UserInfo
		{
			Today = new Structs.DailyScans
			{
				Day = DateTime.Today.Date,
				Scans = 0
			}
		};
		File.WriteAllText(Path.Combine(SettingsDir, FileName), JsonSerializer.Serialize(UserData));
		return 0;
	}

	public static void AddFreeScans(int Scans)
	{
		if (!Directory.Exists(SettingsDir))
		{
			Directory.CreateDirectory(SettingsDir);
		}
		if (File.Exists(Path.Combine(SettingsDir, FileName)))
		{
			Structs.UserInfo UserData2 = JsonSerializer.Deserialize<Structs.UserInfo>(File.ReadAllText(Path.Combine(SettingsDir, FileName)));
			if (UserData2.Today != null)
			{
				if (UserData2.Today.Day == DateTime.Today.Date)
				{
					UserData2.Today.Scans += Scans;
				}
				else
				{
					UserData2.Today = new Structs.DailyScans
					{
						Day = DateTime.Today.Date,
						Scans = 0
					};
				}
			}
			else
			{
				UserData2.Today = new Structs.DailyScans
				{
					Day = DateTime.Today.Date,
					Scans = 0
				};
			}
			File.WriteAllText(Path.Combine(SettingsDir, FileName), JsonSerializer.Serialize(UserData2));
		}
		else
		{
			Structs.UserInfo UserData = new Structs.UserInfo
			{
				Today = new Structs.DailyScans
				{
					Day = DateTime.Today.Date,
					Scans = 0
				}
			};
			File.WriteAllText(Path.Combine(SettingsDir, FileName), JsonSerializer.Serialize(UserData));
		}
	}
}
