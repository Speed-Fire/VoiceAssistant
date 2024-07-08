using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Misc.DictionarySelection;

namespace VoiceAssistant.Misc.Helpers
{
    public class AppearanceHelper(ThemeSelector themeSelector, LanguageSelector languageSelector)
	{
        private readonly ThemeSelector _themeSelector = themeSelector;
        private readonly LanguageSelector _languageSelector = languageSelector;

        public IEnumerable<string> AvailableThemes => _themeSelector.AvailableKeys;
        public IEnumerable<string> AvailableLanguages => _languageSelector.AvailableKeys;
	}
}
