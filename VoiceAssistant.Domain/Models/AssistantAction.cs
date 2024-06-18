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
		[Required]
		public string Name { get; set; } = "";
		[Required]
		public string Command { get; set; } = "";
		public string? Description { get; set; }
		public bool NeedsConfirmation { get; set; }

#nullable disable

		public long AssistantScriptId { get; set; }
		public AssistantScript AssistantScript { get; set; }
	}
}
