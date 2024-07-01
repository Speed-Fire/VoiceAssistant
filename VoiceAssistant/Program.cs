using DBConfiguration.Extensions;
using DBConfiguration.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plugin.Registrator;
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
using VoiceAssistant.Core.Misc;
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

namespace VoiceAssistant
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			InitSubFolders();

			var builder = Host.CreateApplicationBuilder(args);

			// register services
			builder.Services
				.RegisterCore()
				.RegisterSynergyWPFCommon()
				.RegisterSynergyWPFNavigation();

			// Register plugins and default db config values
			var (context, dbConfInitializer) = GetDbConfigInitializer(builder.Configuration);

			RegisterPlugins(builder, dbConfInitializer);
			InitDefaultAppDbConfiguration(dbConfInitializer);

			context.Dispose();

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

		private static (DbContext, IDefaultSettingsInitializer) GetDbConfigInitializer(IConfiguration config)
		{
			var context = new AppDbContext(
				config.GetConnectionString("MainDb") ?? string.Empty);

			var settingsInitializer = new DefaultDbConfigInitializer(context);

			return (context, settingsInitializer);
		}

		private static void RegisterPlugins(HostApplicationBuilder builder,
			IDefaultSettingsInitializer settingsInitializer)
		{
			var pluginFolder = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

			using var registrator = new PluginRegistrator(pluginFolder,
				settingsInitializer);

			registrator.Register(builder.Services, builder.Configuration).Wait();
		}

		private static void InitDefaultAppDbConfiguration(IDefaultSettingsInitializer settingsInitializer)
		{
			var settings = new List<Settings>()
			{
				new("Application:InitializationConfig:Theme", "DarkTheme"),
				new("Application:InitializationConfig:Language", "en-us"),
			};

			settingsInitializer.Initialize(settings);
		}

		private static void InitSubFolders()
		{
			string[] directories = ["Plugins", "Languages", "Themes"];

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
