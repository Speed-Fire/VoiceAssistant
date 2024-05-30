using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Services.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterServices(this IServiceCollection services)
		{
			services
				.AddHostedService<VoiceAssistantService>()
				.AddTransient<SettingsLoadingService>();

			return services;
		}
	}
}
