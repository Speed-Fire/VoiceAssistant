using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Domain.Models
{
    public class AssistantScript
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<Tuple<string, Type>> Parameters { get; set; }

        public long AssistantScriptConstructionId { get; set; }
        public required AssistantScriptConstruction AssistantScriptConstruction { get; set; }

		public long? AssistantScriptAssemblyId { get; set; }
        public AssistantScriptAssembly? AssistantScriptAssembly { get; set; }

		public virtual ICollection<AssistantCommand> Actions { get; set; } = [];
    }
}
