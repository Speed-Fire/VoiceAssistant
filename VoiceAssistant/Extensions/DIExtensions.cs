using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core;
using VoiceAssistant.Misc.DictionarySelection;
using VoiceAssistant.Misc.Options;
using VoiceAssistant.ViewModels;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantActions;

namespace VoiceAssistant.Extensions
{
    internal static class DIExtensions
	{
		public static IServiceCollection RegisterHttpClient(this IServiceCollection services)
		{
			var handler = new SocketsHttpHandler()
			{
				PooledConnectionLifetime = TimeSpan.FromMinutes(2),
			};

			var client = new HttpClient(handler);

			services.AddSingleton(client);

			return services;
		}

		public static IServiceCollection RegisterApp(this IServiceCollection services,
			IConfiguration config)
		{
			services
				.RegisterAppPaths();

			services
				.AddSingleton<LanguageSelector>()
				.AddSingleton<ThemeSelector>();

			services
				.AddSingleton<MainWindow>()
				.AddSingleton<App>();

			services
				.RegisterViewModelss()
				.RegisterViews();

			services
				.ConfigureAppOptions(config);

			return services;
		}

		private static IServiceCollection RegisterAppPaths(this IServiceCollection services)
		{
			var appdata_path =
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					"Synergy.VoiceAssistant");

			var dbs_path = Path.Combine(appdata_path, "Data");

			services
				.AddKeyedSingleton(Consts.APPLICATION_DATA_PATH, appdata_path)
				.AddKeyedSingleton(Consts.APLICATION_DATA_DBS_PATH, dbs_path);

			return services;
		}

		private static IServiceCollection RegisterViews(this IServiceCollection services)
		{
			services
				.AddTransient<MainView>()
				.AddTransient<AssistantActionsView>()
				.AddTransient<ScriptEditorView>();

			return services;
		}

		private static IServiceCollection RegisterViewModelss(this IServiceCollection services)
		{
			services
				.AddTransient<MainVM>()
				.AddTransient<AssistantActionsVM>()
				.AddTransient<ScriptEditorVM>();

			return services;
		}

		private static IServiceCollection ConfigureAppOptions(this IServiceCollection services,
			IConfiguration config)
		{
			services.Configure<InitializationConfig>(config.GetSection("Application:InitializationConfig"));

			return services;
		}
	}
}
