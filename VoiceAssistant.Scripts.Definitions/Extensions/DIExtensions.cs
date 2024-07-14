using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Scripts.Interfaces;

namespace VoiceAssistant.Scripts.Definitions.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterScriptDefinitions(this IServiceCollection services)
		{
			services
				.AddSingleton<IScriptBlockService, ScriptBlockService>();

			return services;
		}
	}
}
