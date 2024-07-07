using Microsoft.Extensions.DependencyInjection;
using Plugin.S2T.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Services.AssistantActionServices;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.Services.Misc.Implementations;
using VoiceAssistant.Services.Misc.Interfaces;
using VoiceAssistant.Services.ProviderInitializers;
namespace VoiceAssistant.Services.Extensions
{
	public class ExceptionNotifier : IExceptionNotifier
	{
		public void Notify(Exception exception)
		{
			
		}
	}

	public static class DIExtensions
	{
		public static int AssistantActionEntity { get; private set; }

		public static IServiceCollection RegisterServices(this IServiceCollection services)
		{
			services
				.AddTransient<IAssistantActionService, AssistantActionService>()
				.AddTransient<IProviderInitializer, AssistantActionsProviderInitializer>()
				.AddTransient<IProviderInitializer, S2TConverterProviderInitializer>();

			services
				.AddSingleton<IVoiceAssistantMonitor, VoiceAssistantMonitor>();

			services
				.AddSingleton<Provider<IS2TConverter>>();

			services
				.AddSingleton<IExceptionNotifier, ExceptionNotifier>();

			services
				.AddScoped<ApplicationSettingsService>();

			return services;
		}
	}
}
