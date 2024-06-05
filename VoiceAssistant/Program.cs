using CSPythonInvoker.Extensions;
using Microsoft.Extensions.DependencyInjection;
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
using VoiceAssistant.ChatGPT.Extensions;
using VoiceAssistant.Extensions;
using VoiceAssistant.Recording.Extensions;
using VoiceAssistant.Services;
using VoiceAssistant.Services.Extensions;

namespace VoiceAssistant
{
	internal class Program
	{
		public static async void Main(string[] args)
		{
			var builder = Host.CreateApplicationBuilder(args);

			builder.Services
				.RegisterCore()
				.RegisterSynergyWPFCommon()
				.RegisterSynergyWPFNavigation()
				.RegisterVoiceRecording(builder.Configuration)
				.RegisterServices()
				.RegisterApp()
				.RegisterHttpClient()
				.RegisterPython()
				.RegisterChatGPT();

			var pluginFolder = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
			using var registrator = new PluginRegistrator(pluginFolder);

			registrator.Register(builder.Services);

			var host = builder.Build();

			var settingsLoader = host.Services.GetRequiredService<SettingsLoadingService>();
			await settingsLoader.LoadAsync();

			var hostrun = host.RunAsync();

			await hostrun;
		}
	}
}
