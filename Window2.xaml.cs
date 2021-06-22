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
using Google.Cloud.Firestore;
using HyperHack;

namespace HyperHack
{
	public partial class Window2 : Window
	{

		public Window2()
		{
			InitializeComponent();
		}
		private void LMBDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}
		private void ExitClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

        private void CopyScript(object sender, RoutedEventArgs e)
        {
			string script = "document.body.firstChild.__vue__.$store._vm.$data.$$state.game.data.roomCode";
			Clipboard.SetText(script);
		}
    }
}
