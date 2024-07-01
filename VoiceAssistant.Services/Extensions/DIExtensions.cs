using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Services.AssistantActionServices;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.Services.Misc.Implementations;
using VoiceAssistant.Services.Misc.Interfaces;
using VoiceAssistant.Services.ProviderInitializers;
namespace VoiceAssistant.Services.Extensions
{
	public static class DIExtensions
	{
		public static int AssistantActionEntity { get; private set; }

		public static IServiceCollection RegisterServices(this IServiceCollection services)
		{
			services
				.AddTransient<IAssistantActionService, AssistantActionService>()
				.AddTransient<IProviderInitializer, AssistantActionsProviderInitializer>();

			services
				.AddSingleton<IVoiceAssistantMonitor, VoiceAssistantMonitor>();

			//services
			//	.AddHostedService<VoiceAssistantService>();

			return services;
		}
	}
}
