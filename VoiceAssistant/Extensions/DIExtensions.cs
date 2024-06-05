using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core;
using VoiceAssistant.ViewModels;

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

		public static IServiceCollection RegisterApp(this IServiceCollection services)
		{
			services
				.RegisterAppPaths();

			services
				.AddSingleton<MainWindow>()
				.AddSingleton<App>()
				.AddTransient<MainVM>();

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
	}
}
