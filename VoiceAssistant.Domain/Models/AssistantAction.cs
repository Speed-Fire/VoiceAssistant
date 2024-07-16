using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Domain.Models
{
    public class AssistantAction
	{
		public long Id { get; set; }
		public required string Name { get; set; } = "";
		public required string Command { get; set; } = "";
		public required string Input { get; set; } = "";
		public string? Description { get; set; }
		public bool NeedsConfirmation { get; set; }
		public bool IsEnabled { get; set; } = true;

		public long? AssistantScriptId { get; set; }
		public AssistantScript? AssistantScript { get; set; }
	}
}
