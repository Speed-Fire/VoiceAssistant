using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Scripts.Compilation.Models
{
	internal class ScriptTree(EntryNode entry, IReadOnlyList<ScriptBlockNode> nodes)
	{
		public EntryNode Entry { get; } = entry;
		public IReadOnlyList<ScriptBlockNode> Nodes { get; } = nodes;
	}
}
