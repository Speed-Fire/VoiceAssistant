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

namespace VoiceAssistant.CommandResolving.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterCommandResolving(
			this IServiceCollection services,
			IConfiguration config)
		{
			services
				.Configure<CommandResolverOptions>(config.GetSection("Application:CommandResolverOptions"));

			services
				.AddTransient<CommandResolver>()
				.AddTransient<ICommandResolver, DummyCommandResolver>()
				.AddTransient<ICommandResolver, SmartCommandResolver>();

			var actProvider = new Provider<List<AssistantAction>>
			{
				Value = []
			};

			services
				.AddSingleton(actProvider)
				.AddSingleton<IActiveCommandResolverSwitch, ActiveCommandResolverSwitch>();

			services
				.AddSingleton<Func<IChatGPT?>>(provider => () => provider.GetService<IChatGPT>())
				.AddSingleton<Func<IEnumerable<ICommandResolver>>>(provider => 
					() => provider.GetServices<ICommandResolver>());

			return services;
		}
	}
}
