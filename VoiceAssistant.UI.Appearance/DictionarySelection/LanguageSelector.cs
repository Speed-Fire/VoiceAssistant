using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.UI.Appearance.DictionarySelection
{
    public class LanguageSelector : ResourceDictionarySelector
    {
        public LanguageSelector()
            :
            base([
				"pack://application:,,,/VoiceAssistant.UI.Appearance;component/Languages/en-us.xaml"
				])
        {
        }
    }
}
