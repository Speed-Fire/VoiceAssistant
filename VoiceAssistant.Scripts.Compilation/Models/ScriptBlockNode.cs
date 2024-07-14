using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Scripts.Models;

namespace VoiceAssistant.Scripts.Compilation.Models
{
	internal class ScriptBlockNode(
		ScriptBlock scriptBlock,
		IReadOnlyList<string> inputValues)
	{
		public ScriptBlock ScriptBlock { get; } = scriptBlock;
		public IReadOnlyList<string> InputValues { get; } = inputValues;
	}
}
