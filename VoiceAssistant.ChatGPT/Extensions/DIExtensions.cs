using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;

namespace VoiceAssistant.ChatGPT.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterChatGPT(this IServiceCollection services,
			IConfiguration config)
		{
			var apikey = config.GetRequiredSection("Gemini")
				.GetRequiredSection("ServiceApiKey").Value ?? string.Empty;
			
#pragma warning disable SKEXP0070 // Тип предназначен только для оценки и может быть изменен или удален в будущих обновлениях. Чтобы продолжить, скройте эту диагностику.
			services
				.AddGoogleAIGeminiChatCompletion("gemini-1.5-flash", apikey);
#pragma warning restore SKEXP0070 // Тип предназначен только для оценки и может быть изменен или удален в будущих обновлениях. Чтобы продолжить, скройте эту диагностику.

			services
				.AddTransient<IChatGPT, ChatGPT>();

			return services;
		}
	}
}
