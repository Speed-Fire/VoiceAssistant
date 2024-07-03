using Plugin.Base;
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
using VoiceAssistant.Entities;
using VoiceAssistant.Views.Plugins;

namespace VoiceAssistant.ViewModels.Plugins
{
	internal class PluginsVM : ViewModel<PluginsView>
	{
        public List<PluginInfoEntity> Plugins { get; } = [];

        public PluginsVM(Provider<IEnumerable<PluginInfoEntity>> plugins)
        {
            if (plugins.Value is not null)
                Plugins = new(plugins.Value);
            else
                Plugins = [];
        }
    }
}
