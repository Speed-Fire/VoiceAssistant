using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Scripts.Compilation.Parsers;
using VoiceAssistant.Scripts.Interfaces;

namespace VoiceAssistant.Scripts.Compilation.Extensions
{
	public static class DIExtensions
	{
		public static IServiceCollection RegisterScriptCompilation(this IServiceCollection services)
		{
			services
				.AddTransient<JsonToScriptBlocksParser>()
				.AddTransient<IScriptCompiler, ScriptCompiler>();

			return services;
		}
	}
}
