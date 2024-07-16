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
using VoiceAssistant.ViewModels.Settings;
using VoiceAssistant.Views;

namespace VoiceAssistant.ViewModels
{
    public partial class MainVM(
			[FromKeyedServices(NavConsts.SINGLETON_SERVICE)] INavigationService globalNavigation,
			IVoiceAssistantMonitor voiceAssistantMonitor) 
		: ViewModel
	{
		private readonly INavigationService _globalNavigation = globalNavigation;
		private readonly IVoiceAssistantMonitor _voiceAssistantMonitor = voiceAssistantMonitor;

		#region Navigation

		[RelayCommand]
		private void OpenCommandsTab()
		{
			_voiceAssistantMonitor.Lock();

            _globalNavigation.NavigateTo<AssistantCommandsVM>();
        }

		[RelayCommand]
		private void OpenScriptEditorTab()
		{
			_voiceAssistantMonitor.Unlock();

			_globalNavigation.NavigateTo<ScriptEditorVM>();
		}

		[RelayCommand]
		private void OpenPluginsTab()
		{
			_voiceAssistantMonitor.Unlock();

			_globalNavigation.NavigateTo<PluginsVM>();
		}

		[RelayCommand]
		private void OpenSettingsTab()
		{
			_voiceAssistantMonitor.Lock();

			_globalNavigation.NavigateTo<SettingsVM>();
		}

		#endregion
	}
}
