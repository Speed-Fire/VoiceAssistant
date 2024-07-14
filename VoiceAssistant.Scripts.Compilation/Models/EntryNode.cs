using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Scripts.Compilation.Models
{
	internal class EntryNode(IReadOnlyList<Tuple<string, Type>> entryParameters)
	{
		public IReadOnlyList<Tuple<string, Type>> EntryParamaters { get; } = entryParameters;
	}
}
