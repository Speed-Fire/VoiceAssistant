using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.UI.Appearance.DictionarySelection;
using VoiceAssistant.UI.Appearance.Options;

namespace VoiceAssistant.UI.Appearance.Helpers
{
    internal class AppearanceSettingsHelper(
        ThemeSelector themeSelector,
        LanguageSelector languageSelector,
        IOptionsMonitor<AppearanceOptions> options)
        : IAppearanceSettingsHelper
	{
        public IEnumerable<string> AvailableThemes => themeSelector.AvailableKeys.ToList();
        public IEnumerable<string> AvailableLanguages => languageSelector.AvailableKeys.ToList();
        public string? SelectedTheme => options.CurrentValue.Theme;
		public string? SelectedLanguage => options.CurrentValue.Language;
	}
}
