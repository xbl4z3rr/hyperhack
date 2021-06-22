// QuizizzSupport.Structs
using System;
using System.Collections.Generic;
using HyperHack;

internal class Structs
{
	public class KeyValidation
	{
		public bool success { get; set; }

		public int Devices { get; set; }

		public int Quizzes { get; set; }

		public int Days { get; set; }

		public KeyType keyType { get; set; }
	}

	public enum KeyType
	{
		Dev,
		Qui,
		Day,
		DevQui,
		DevDay,
		QuiDay,
		DevQuizDay,
		none
	}

	public enum ValidationResponse
	{
		WrongKey,
		CorrectKey,
		NewDevice,
		NewKey
	}

	public class UserInfo
	{
		public List<KeyInformation> Keys { get; set; }

		public DailyScans Today { get; set; }
	}

	public class KeyInformation
	{
		public string Key { get; set; }

		public int QuizzesScanned { get; set; }

		public int MaxQuizzes { get; set; }
	}

	public class DailyScans
	{
		public DateTime Day { get; set; }

		public int Scans { get; set; }
	}

	public class GetQuizResp
	{
		public bool Success { get; set; }

		public Utils.Quiz Quiz { get; set; }
	}
}
