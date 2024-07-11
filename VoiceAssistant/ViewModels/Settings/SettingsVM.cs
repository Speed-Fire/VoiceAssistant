using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Misc;
using VoiceAssistant.Views.Settings;

namespace VoiceAssistant.ViewModels.Settings
{
	internal partial class SettingsVM(
			Func<object, INavigationService> navigationServiceFactory) 
		: ViewModel<SettingsView>
	{
		private readonly INavigationService _localNavigation =
			navigationServiceFactory.Invoke(NavigationChannels.SETTINGS_CHANNEL);

		[RelayCommand]
		private void OpenAppearanceSettings()
		{
			_localNavigation.NavigateTo<AppearanceSettingsVM>();
        }

		[RelayCommand]
		private void OpenVoiceRecognitionSettings()
		{
			_localNavigation.NavigateTo<VoiceRecognitionSettingsVM>();
		}

		[RelayCommand]
		private void OpenSpeechSynthesisSettings()
		{
			_localNavigation.NavigateTo<SpeechSynthesisSettingsVM>();
		}

		public override void Dispose()
		{
			_localNavigation.Dispose();
		}
	}
}
