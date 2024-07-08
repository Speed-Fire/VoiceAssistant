using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Components;
using VoiceAssistant.Core;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.HostedServices;
using VoiceAssistant.Misc;
using VoiceAssistant.Notifications;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.ViewModels;
using VoiceAssistant.ViewModels.Components;
using VoiceAssistant.ViewModels.Plugins;
using VoiceAssistant.ViewModels.ScriptEditing;
using VoiceAssistant.ViewModels.Settings;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantActions;
using VoiceAssistant.Views.Plugins;
using VoiceAssistant.Views.Settings;

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
				.AddHostedService<WpfStarterService>();

			services
				.RegisterAppPaths();

			services
				.AddSingleton<MainWindow>()
				.AddSingleton<App>();

			services
				.RegisterViewServices()
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
				.AddTransient<AssistantActionsView>()
				.AddTransient<ScriptEditorView>()
				.AddTransient<VoiceAssistantListeningStatusComponent>()
				.AddTransient<PluginsView>()
				.AddTransient<SettingsView>()
				.AddScoped<AppearanceSettingsView>();

			return services;
		}

		private static IServiceCollection RegisterViewModelss(this IServiceCollection services)
		{
			services
				.AddTransient<MainVM>()
				.AddTransient<AssistantActionsVM>()
				.AddTransient<ScriptEditorVM>()
				.AddTransient<VoiceAssistantListeningStatusVM>()
				.AddTransient<PluginsVM>()
				.AddTransient<SettingsVM>()
				.AddScoped<AppearanceSettingsVM>();

			return services;
		}

		private static IServiceCollection RegisterViewServices(this IServiceCollection services)
		{

			services
				.AddSingleton<UrgentNotificationService>()
				.AddHostedService((provider) => 
					{
						return provider.GetRequiredService<UrgentNotificationService>();
					})
				.AddSingleton<IUrgentNotifier, UrgentNotifier>()
				.AddTransient<UrgentNotificatorComponent>();

			return services;
		}

		private static IServiceCollection ConfigureAppOptions(this IServiceCollection services,
			IConfiguration config)
		{
			

			return services;
		}
	}
}
