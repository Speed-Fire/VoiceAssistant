using DBConfiguration.Extensions;
using DBConfiguration.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plugin.S2T.Base;
using Synergy.Core;
using Synergy.WPF.Common.Extensions;
using Synergy.WPF.Navigation.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Extensions;
using VoiceAssistant.ChatGPT.Extensions;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Models;
using VoiceAssistant.DAL.Extensions;
using VoiceAssistant.SpeechSynthesis.Extensions;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.Extensions;
using VoiceAssistant.Recording.Extensions;
using VoiceAssistant.Services;
using VoiceAssistant.Services.Extensions;
using VoiceAssistant.Services.Misc.Interfaces;
using PluginsSystem;
using System.Globalization;
using Microsoft.Extensions.Logging;
using VoiceAssistant.HostedServices;
using VoiceAssistant.UI.Appearance.Extensions;
using VoiceAssistant.UI.Appearance.HostedServices;
using VoiceAssistant.Services.Hosted;

namespace VoiceAssistant
{
    internal class Program
	{
		public static async Task Main(string[] args)
		{
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			InitSubFolders();

			var builder = Host.CreateApplicationBuilder(args);

			var loggerFactory = LoggerFactory.Create(logBuilder =>
			{
				logBuilder
					.AddConfiguration(builder.Configuration)
					.AddConsole()
					.AddEventLog()
					.SetMinimumLevel(LogLevel.Information);
			});

			// register services
			builder.Services
				.RegisterCore()
				.RegisterSynergyWPFCommon()
				.RegisterSynergyWPFNavigation();

			// Register plugins
			await LoadPlugins(builder, loggerFactory);

			// continue on service registration
			builder.Services
				.RegisterDAL(builder.Configuration)
				.RegisterVoiceRecording(builder.Configuration)
				.RegisterSpeechSynthesis(builder.Configuration)
				.RegisterServices(builder.Configuration)
				.RegisterApp(builder.Configuration)
				.RegisterHttpClient()
				.RegisterChatGPT(builder.Configuration)
				.RegisterCommandResolving(builder.Configuration)
				.RegisterAppearance(builder.Configuration);

			RegisterHostedServices(builder.Services);

			// host building
			var host = builder.Build();

			await host.RunAsync();
		}

		private static Task LoadPlugins(
			HostApplicationBuilder builder,
			ILoggerFactory loggerFactory)
		{
			var pluginFolder = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
			var configFolder = Path.Combine(Directory.GetCurrentDirectory(), "Config");

			var pluginLoader = new PluginLoader(loggerFactory);

			return pluginLoader.LoadAsync(
				builder.Services,
				builder.Configuration,
				pluginFolder,
				configFolder);
		}

		private static void InitSubFolders()
		{
			string[] directories = ["Plugins", "Languages", "Themes", "Config"];

			foreach(var directory in directories)
			{
				if(!Directory.Exists(directory))
					Directory.CreateDirectory(directory);
			}
		}

		private static void RegisterHostedServices(IServiceCollection services)
		{
			services
				.AddHostedService<ProviderInitializationService>()
				.AddHostedService<S2TConverterService>()
				.AddHostedService<VoiceAssistantService>()
				.AddHostedService<AppearanceService>()
				.AddHostedService<WpfStarterService>();
		}
	}
}
