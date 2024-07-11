using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.SettingsHelpers
{
	public interface IAppearanceSettingsHelper
	{
		IEnumerable<string> AvailableThemes { get; }
		IEnumerable<string> AvailableLanguages { get; }

		string? SelectedTheme { get; }
		string? SelectedLanguage { get; }
	}
}
