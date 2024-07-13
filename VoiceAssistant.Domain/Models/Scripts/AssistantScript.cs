using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Domain.Models.Scripts
{
	public abstract class AssistantScript
	{
		public required long Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
	}
}
