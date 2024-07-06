using Plugin.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsSystem.Models
{
	internal class PluginData(PluginInfo pluginInfo, IDIPluginRegistrator registrator)
	{
		public PluginInfo PluginInfo { get; } = pluginInfo;
		public IDIPluginRegistrator Registrator { get; } = registrator;
	}
}
