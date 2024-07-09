using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Views.Settings;

namespace VoiceAssistant.ViewModels.Settings
{
	internal partial class SettingsVM(
			Func<object, INavigationService> navigationServiceFactory) 
		: ViewModel<SettingsView>
	{
		private readonly INavigationService _localNavigation =
			navigationServiceFactory.Invoke("Settings");

		[RelayCommand]
		private void OpenAppearanceSettings()
		{
			_localNavigation.NavigateTo<AppearanceSettingsVM>();
        }

		[RelayCommand]
		private void OpenVoiceRecognitionSettings()
		{

		}

		public override void Dispose()
		{
			_localNavigation.Dispose();
		}
	}
}
