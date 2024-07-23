using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Base;
using PluginsSystem.Entities;
using PluginsSystem.Settings.ViewModels;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.Misc.Interfaces;
using VoiceAssistant.Views.Plugins;

namespace VoiceAssistant.ViewModels.Plugins
{
	internal partial class PluginsVM : ViewModel<PluginsView>
	{
        private readonly IServiceProvider _serviceProvider;
        private readonly IUrgentNotifier _urgentNotifier;
		private readonly IVoiceAssistantMonitor _voiceAssistantMonitor;

		public List<PluginInfoEntity> Plugins { get; } = [];

		public PluginsVM(
            IServiceProvider serviceProvider,
			IUrgentNotifier urgentNotifier,
			IVoiceAssistantMonitor voiceAssistantMonitor,
			Provider<IEnumerable<PluginInfoEntity>> plugins)
		{
            _serviceProvider = serviceProvider;
			_urgentNotifier = urgentNotifier;
			_voiceAssistantMonitor = voiceAssistantMonitor;

			if (plugins.Value is not null)
				Plugins = new(plugins.Value);
			else
				Plugins = [];
		}

		[RelayCommand(CanExecute = nameof(CanOpenPluginSettingsExecute))]
        private void OpenPluginSettings(PluginInfoEntity pluginInfo)
        {
            if (pluginInfo is null)
                return;

            try
            {
                _voiceAssistantMonitor.Lock();

                var vm = ActivatorUtilities
                    .CreateInstance<PluginSettingsVM>(_serviceProvider, [pluginInfo]);

                this.Navigation.PushDialog(vm, _ => { _voiceAssistantMonitor.Unlock(); });
			}
            catch
            {
                _urgentNotifier.NotifyError("Can't open plugin settings. Plugin config is incorrect!");

                _voiceAssistantMonitor.Unlock();
            }
        }

        private static bool CanOpenPluginSettingsExecute(PluginInfoEntity pluginInfo)
        {
            if (pluginInfo is null)
                return false;

            return pluginInfo.Parameters.Count > 0;
        }
    }
}
