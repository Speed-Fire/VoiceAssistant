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
using VoiceAssistant.Services.Hosted;
using Microsoft.Extensions.Configuration;
using VoiceAssistant.Services.Options;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.Services.Helpers;
using VoiceAssistant.Services.Initializers;
using VoiceAssistant.Services.Misc;

namespace VoiceAssistant.Services.Extensions
{
    public static class DIExtensions
	{
		public static IServiceCollection RegisterServices(
			this IServiceCollection services,
			IConfiguration config)
		{
			services
				.AddTransient<IAssistantActionService, AssistantActionService>()
				.AddTransient<AssistantActionsProviderInitializer>()
				.AddTransient<SequentialInitializerQueue>();

			services
				.AddSingleton<IVoiceAssistantMonitor, VoiceAssistantMonitor>();

			services
				.AddSingleton<Provider<IS2TConverter>>()
				.AddSingleton<S2TConverterService>();

			services
				.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>()
				.AddTransient<SpeechToTextService>()
				.AddTransient<IS2TConverterSettingsHelper, S2TConverterSettingsHelper>();

			services.
				Configure<SpeechToTextOptions>(config.GetSection("Application:SpeechToText"));

			return services;
		}
	}
}
