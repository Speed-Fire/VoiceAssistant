using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Interfaces
{
	public interface IScriptBlockService
	{
		IEnumerable<ScriptBlockDefinition> ScriptBlockDefinitions { get; }
		IEnumerable<ScriptBlock> ScriptBlocks { get; }
	}
}
