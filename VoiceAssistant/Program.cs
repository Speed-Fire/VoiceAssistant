using Microsoft.Extensions.Hosting;
using Plugin.Registrator;
using Synergy.Core;
using Synergy.WPF.Common.Extensions;
using Synergy.WPF.Navigation.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Extensions;
using VoiceAssistant.Recording.Extensions;

namespace VoiceAssistant
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var builder = Host.CreateApplicationBuilder(args);

			builder.Services
				.RegisterCore()
				.RegisterSynergyWPFCommon()
				.RegisterSynergyWPFNavigation()
				.RegisterVoiceRecording(builder.Configuration)
				.RegisterApp();

			var pluginFolder = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
			using var registrator = new PluginRegistrator(pluginFolder);

			registrator.Register(builder.Services);

			var host = builder.Build();
			host.RunAsync();
		}
	}
}
