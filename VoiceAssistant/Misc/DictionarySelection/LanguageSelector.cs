using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Misc.DictionarySelection
{
    public class LanguageSelector : ResourceDictionarySelector
    {
        public LanguageSelector()
            :
            base([
                "pack://application:,,,/VoiceAssistant;component/Languages/en-us.xaml"
                ])
        {
        }
    }
}
