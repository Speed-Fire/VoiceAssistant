using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Misc;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.ActionManagement.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterCommandResolving(this IServiceCollection services)
		{
			services
				.AddTransient<ICommandResolver, CommandResolver>()
				.AddTransient<DummyCommandResolver>()
				.AddTransient<SmartCommandResolver>();

			var actProvider = new Provider<List<AssistantAction>>
			{
				Value = []
			};

			services
				.AddSingleton(actProvider);

			return services;
		}
	}
}
