using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.UI.Styles
{
	public partial class StylesSharedDictionary
	{
		private static readonly StylesSharedDictionary _instance = new();
		public static StylesSharedDictionary Instance
		{
			get => _instance;
		}

        private StylesSharedDictionary()
        {
			InitializeComponent();
        }
    }
}
