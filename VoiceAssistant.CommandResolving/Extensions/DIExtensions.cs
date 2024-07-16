using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.CommandResolving.Options;
using VoiceAssistant.CommandResolving.Switch;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers.Commands;
using VoiceAssistant.CommandResolving.TextResolving.Services;
using VoiceAssistant.CommandResolving.TextResolving.Resolvers.Confirmation;
using VoiceAssistant.Domain.Underlying;

namespace VoiceAssistant.CommandResolving.Extensions
{
    public static class DIExtensions
	{
		public static IServiceCollection RegisterCommandResolving(
			this IServiceCollection services,
			IConfiguration config)
		{
			services
				.Configure<TextResolvingOptions>(config.GetSection("Application:TextResolving"));

			services
				.AddTransient<ITextResolvingService, TextResolvingService>()
				.AddTransient<ActiveTextResolverSwitch<bool?>, ActiveConfirmationResolverSwitch>()
				.AddTransient<ActiveTextResolverSwitch<UnderlyingCommand>, ActiveCommandResolverSwitch>()
				.AddTransient<SmartCommandResolver>()
				.AddTransient<DummyCommandResolver>()
				.AddTransient<SmartConfirmationResolver>()
				.AddTransient<DummyConfirmationResolver>();

			var actProvider = new Provider<List<AssistantAction>>
			{
				Value = []
			};

			services
				.AddSingleton(actProvider);

			services
				.AddSingleton<Func<IChatGPT?>>(provider => () => provider.GetService<IChatGPT>());

			return services;
		}
	}
}
