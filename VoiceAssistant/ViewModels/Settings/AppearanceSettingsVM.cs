using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Services;
using VoiceAssistant.UI.Appearance.Helpers;
using VoiceAssistant.UI.Appearance.Options;
using VoiceAssistant.Views.Settings;

namespace VoiceAssistant.ViewModels.Settings
{
	internal partial class AppearanceSettingsVM
		: ViewModel<AppearanceSettingsView>
	{
		private readonly IUrgentNotifier _urgentNotifier;
		private readonly ApplicationSettingsService _settingsService;

		[ObservableProperty]
		private IEnumerable<string> _availableThemes;

		[ObservableProperty]
		private IEnumerable<string> _availableLanguages;

		[ObservableProperty]
		private string? _selectedTheme;

		[ObservableProperty]
		private string? _selectedLanguage;

		public AppearanceSettingsVM(
			IUrgentNotifier urgentNotifier,
			ApplicationSettingsService settingsService, 
			AppearanceHelper appearanceHelper,
			IOptions<AppearanceOptions> options)
		{
			_urgentNotifier = urgentNotifier;
			_settingsService = settingsService;
			
			_availableThemes = appearanceHelper.AvailableThemes;
			_availableLanguages = appearanceHelper.AvailableLanguages;

			_selectedTheme = _availableThemes.FirstOrDefault(t => t == options.Value.Theme);
			_selectedLanguage = _availableLanguages.FirstOrDefault(l => l == options.Value.Language);

			_settingsService.SetCurrentSection("Appearance");
		}

		async partial void OnSelectedLanguageChanged(string? oldValue, string? newValue)
		{
			await Task.Run(async () =>
			{
				if (string.IsNullOrEmpty(oldValue))
					return;

				var language = newValue!;

				var result = await _settingsService.SetValueAsync("Language", language);

				if (result != null)
				{
					_urgentNotifier.NotifyError("Can't change language!", exception: result);
				}
			});
		}

		async partial void OnSelectedThemeChanged(string? oldValue, string? newValue)
		{
			await Task.Run(async () =>
			{
				if (string.IsNullOrEmpty(oldValue))
					return;

				var theme = newValue!;

				var result = await _settingsService.SetValueAsync("Theme", theme);

				if (result != null)
				{
					_urgentNotifier.NotifyError("Can't change theme!", exception: result);
				}
			});
		}
	}
}
