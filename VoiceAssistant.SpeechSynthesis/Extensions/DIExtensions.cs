using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.SettingsHelpers;
using VoiceAssistant.SpeechSynthesis.Helpers;
using VoiceAssistant.SpeechSynthesis.Options;
using VoiceAssistant.SpeechSynthesis.Services;

namespace VoiceAssistant.SpeechSynthesis.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterSpeechSynthesis(
			this IServiceCollection services,
			IConfiguration config)
		{
			services
				.Configure<AssistantVoiceOptions>(config.GetSection("Application:AssistantVoice"));

			services
				.AddSingleton<IAssistantVoice, AssistantVoice>()
				.AddTransient<IAssistantVoiceSettingsHelper, AssistantVoiceSettingsHelper>()
				.AddTransient<SynthesizerVoiceSelectingService>();

			return services;
		}
	}
}
