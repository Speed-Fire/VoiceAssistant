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
using VoiceAssistant.ActionManagement.Extensions;
using VoiceAssistant.ChatGPT.Extensions;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.Core.Models;
using VoiceAssistant.DAL.Extensions;
using VoiceAssistant.DAL.Providers;
using VoiceAssistant.Extensions;
using VoiceAssistant.Misc;
using VoiceAssistant.Misc.DictionarySelection;
using VoiceAssistant.Recording.Extensions;
using VoiceAssistant.Services;
using VoiceAssistant.Services.Extensions;
using VoiceAssistant.Services.Misc.Interfaces;
using PluginsSystem;
using System.Globalization;
using Microsoft.Extensions.Logging;

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
				.RegisterServices()
				.RegisterApp(builder.Configuration)
				.RegisterHttpClient()
				.RegisterChatGPT(builder.Configuration)
				.RegisterCommandResolving(builder.Configuration);

			builder.Services.AddHostedService<WpfStarter>();

			// host building
			var host = builder.Build();

			// Application Appearance initialization
			InitThemes(host.Services);
			InitLanguages(host.Services);

			await InitProviders(host.Services);

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

		private static void InitThemes(IServiceProvider services)
		{
			var themeSelector = services.GetRequiredService<ThemeSelector>();

			var entries = Directory.GetFiles("Themes", "*.xaml");
			foreach (var entry in entries)
			{
				themeSelector.AddSourcePath(entry);
			}
		}

		private static void InitLanguages(IServiceProvider services)
		{
			var languageSelector = services.GetRequiredService<LanguageSelector>();

			var entries = Directory.GetFiles("Languages", "*.xaml");
			foreach (var entry in entries)
			{
				languageSelector.AddSourcePath(entry);
			}
		}

		private static async Task InitProviders(IServiceProvider services)
		{
			var initializers = services.GetServices<IProviderInitializer>();

			foreach(var initializer in initializers)
			{
				await initializer.InitializeAsync();
			}
		}
	}
}
