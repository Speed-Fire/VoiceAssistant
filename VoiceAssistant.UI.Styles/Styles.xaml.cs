using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace VoiceAssistant.UI.Styles
{
	public partial class StylesDictionary
	{
		private static readonly StylesDictionary _instance = new();
		public static StylesDictionary Instance
		{
			get => _instance;
		}

		private StylesDictionary()
		{
			InitializeComponent();
		}
	}
}
