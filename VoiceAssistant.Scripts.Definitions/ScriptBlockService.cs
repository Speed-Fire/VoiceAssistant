using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Scripts.Definitions.Models;
using VoiceAssistant.Scripts.Interfaces;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Definitions
{
	internal class ScriptBlockService : IScriptBlockService
	{
		private readonly List<ScriptBlock> _scriptBlocks = 
			[
				new DelayScriptBlock(),
				new SpeakScriptBlock(),
				new TestScriptBlock()
			];

		public IEnumerable<ScriptBlockDefinition> ScriptBlockDefinitions => _scriptBlocks;
		public IEnumerable<ScriptBlock> ScriptBlocks => _scriptBlocks;
	}
}
