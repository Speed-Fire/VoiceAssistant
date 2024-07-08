using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Misc.Helpers;
using VoiceAssistant.Services;
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
			AppearanceHelper appearanceHelper)
		{
			_urgentNotifier = urgentNotifier;
			_settingsService = settingsService;
			
			_availableThemes = appearanceHelper.AvailableThemes;
			_availableLanguages = appearanceHelper.AvailableLanguages;

			_settingsService.SetCurrentSection("Appearance");
		}

		[RelayCommand]
		private async Task OnLoaded()
		{
			await Task.Run(async () =>
			{
				await InitSelectedLanguage();
				await InitSelectedTheme();
			});
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

		#region Selection initialization

		private async Task InitSelectedTheme()
		{
			var currentThemeResult = await _settingsService.GetValueAsync("Theme");
			if (!currentThemeResult.IsFirst)
			{
				_urgentNotifier.NotifyError("Can't get current theme!",
					exception: currentThemeResult.Second);
			}
			else
			{
				SelectedTheme = AvailableThemes
					.FirstOrDefault<string?>(t => t == currentThemeResult.First);
			}
		}

		private async Task InitSelectedLanguage()
		{
			var currentLanguageResult = await _settingsService.GetValueAsync("Language");
			if (!currentLanguageResult.IsFirst)
			{
				_urgentNotifier.NotifyError("Can't get current language!",
					exception: currentLanguageResult.Second);
			}
			else
			{
				SelectedLanguage = AvailableLanguages
					.FirstOrDefault<string?>(t => t == currentLanguageResult.First);
			}
		}

		#endregion
	}
}
