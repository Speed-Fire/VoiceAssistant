using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Misc;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Views;

namespace VoiceAssistant.ViewModels
{
	public partial class MainVM : ViewModel<MainView>
	{
		private readonly INavigationService _localNavigation;

		public MainVM(
			[FromKeyedServices(NavConsts.SCOPED_SERVICE)] INavigationService localNavigation)
		{
			_localNavigation = localNavigation;
		}

		#region Navigation

		[RelayCommand]
		private void OpenCommandsTab()
		{
            _localNavigation.NavigateTo<CommandsVM>();
        }

		[RelayCommand]
		private void OpenScriptEditorTab()
		{
			_localNavigation.NavigateTo<ScriptEditorVM>();
		}

		[RelayCommand]
		private void OpenPluginsTab()
		{

		}

		[RelayCommand]
		private void OpenSettingsTab()
		{

		}

		#endregion
	}
}
