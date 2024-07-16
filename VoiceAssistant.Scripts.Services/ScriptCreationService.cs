using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Common;
using VoiceAssistant.Domain.Underlying;
using VoiceAssistant.Scripts.Interfaces;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Services
{
	internal class ScriptCreationService(
		IUnderlyingScriptFactory scriptFactory,
		IScriptCompiler compiler)
	{
		private readonly IUnderlyingScriptFactory _scriptFactory = scriptFactory;
		private readonly IScriptCompiler _compiler = compiler;

		public async Task<OneOf<UnderlyingScript, Exception>> Create(string json)
		{
			var streamResult = await _compiler.Compile(json);
			if (!streamResult.IsFirst)
				return streamResult.Second;

			var underlyingScriptResult = _scriptFactory.Create(streamResult.First, 0);
			
			return underlyingScriptResult;
		}
	}
}
