using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.ViewModels;

namespace VoiceAssistant.Extensions
{
	internal static class DIExtensions
	{
		public static IServiceCollection RegisterApp(this IServiceCollection services)
		{
			services
				.AddSingleton<MainWindow>()
				.AddSingleton<App>()
				.AddTransient<MainVM>();

			return services;
		}
	}
}
