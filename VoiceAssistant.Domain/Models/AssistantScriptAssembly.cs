using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Domain.Models
{
	public class AssistantScriptAssembly
	{
		public long Id { get; set; }
		public required byte[] RawAssembly { get; set; }
	}
}
