using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using HyperHack;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace HyperHack
{
	public partial class App : Application
	{

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "5.0.3.0")]
		public void InitializeComponents()
		{
			base.StartupUri = new Uri("Window1.xaml", UriKind.Relative);
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "5.0.3.0")]
		public static void AppMain()
		{
			App app = new App();
			app.InitializeComponents();
			app.Run();
		}
	}
}
