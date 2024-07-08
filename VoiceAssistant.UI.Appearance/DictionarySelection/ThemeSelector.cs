using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.UI.Appearance.DictionarySelection
{
    public class ThemeSelector : ResourceDictionarySelector
    {
        public ThemeSelector()
            :
            base([
				"pack://application:,,,/VoiceAssistant.UI.Appearance;component/Themes/DarkTheme.xaml",
				"pack://application:,,,/VoiceAssistant.UI.Appearance;component/Themes/LightTheme.xaml"
				])
        {
        }
    }
}
