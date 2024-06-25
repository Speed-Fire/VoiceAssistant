using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Misc.DictionarySelection
{
    internal class ThemeSelector : ResourceDictionarySelector
    {
        public ThemeSelector()
            :
            base([
                "pack://application:,,,/VoiceAssistant;component/Themes/DarkTheme.xaml",
                "pack://application:,,,/VoiceAssistant;component/Themes/LightTheme.xaml"
                ])
        {
        }
    }
}
