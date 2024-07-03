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
using VoiceAssistant.Services.Misc.Interfaces;
using VoiceAssistant.ViewModels.Plugins;
using VoiceAssistant.ViewModels.ScriptEditing;
using VoiceAssistant.Views;

namespace VoiceAssistant.ViewModels
{
    public partial class MainVM : ViewModel<MainView>
	{
		private readonly INavigationService _localNavigation;
		private readonly IVoiceAssistantMonitor _voiceAssistantMonitor;

		public MainVM(
			[FromKeyedServices(NavConsts.SCOPED_SERVICE)] INavigationService localNavigation,
			IVoiceAssistantMonitor voiceAssistantMonitor)
		{
			_localNavigation = localNavigation;
			_voiceAssistantMonitor = voiceAssistantMonitor;
		}

		#region Navigation

		[RelayCommand]
		private void OpenCommandsTab()
		{
			_voiceAssistantMonitor.Lock();

            _localNavigation.NavigateTo<AssistantActionsVM>();
        }

		[RelayCommand]
		private void OpenScriptEditorTab()
		{
			_voiceAssistantMonitor.Unlock();

			_localNavigation.NavigateTo<ScriptEditorVM>();
		}

		[RelayCommand]
		private void OpenPluginsTab()
		{
			_voiceAssistantMonitor.Unlock();

			_localNavigation.NavigateTo<PluginsVM>();
		}

		[RelayCommand]
		private void OpenSettingsTab()
		{
			_voiceAssistantMonitor.Lock();
		}

		#endregion
	}
}
