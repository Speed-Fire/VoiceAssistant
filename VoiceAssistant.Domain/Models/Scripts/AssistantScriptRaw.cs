using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Domain.Models.Scripts
{
	public class AssistantScriptRaw : AssistantScript
	{
		public required string ConstructionString { get; set; }
	}
}
