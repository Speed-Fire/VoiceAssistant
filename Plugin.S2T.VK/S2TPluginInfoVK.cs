using Plugin.Base;
using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.S2T.VK
{
	internal class S2TPluginInfoVK() : PluginInfo(
				  "VK Converter",
				  "Supports only Russian language.",
				  "1.0.0",
				  "Sidorovich Vladislav",
				  "https://github.com/Speed-Fire/VoiceAssistant",
				  Assembly
					.GetExecutingAssembly()
					.GetManifestResourceStream("Plugin.S2T.VK.Resources.vkontakte64.png"))
	{
	}
}
