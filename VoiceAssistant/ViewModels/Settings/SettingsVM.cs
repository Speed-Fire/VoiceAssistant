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
			[FromKeyedServices(Synergy.WPF.Navigation.Misc.NavConsts.SCOPED_SERVICE)]
				INavigationService localNavigation) 
		: ViewModel<SettingsView>
	{
		private readonly INavigationService _localNavigation = localNavigation;

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
#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
			_localNavigation.NavigateTo(null);
#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
		}
	}
}
