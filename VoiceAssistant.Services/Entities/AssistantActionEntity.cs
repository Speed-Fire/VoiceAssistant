using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.Services.Entities
{
	public partial class AssistantActionEntity : PublicValidator
	{
		public long Id { get; set; } = 0;
		public long AssistantScriptId { get; set; } = 0;

		[ObservableProperty]
		private string _name = string.Empty;

		[ObservableProperty]
		private string _command = string.Empty;

		[ObservableProperty]
		private string? _description;

		[ObservableProperty]
		private bool _needsConfirmation;

		[ObservableProperty]
		private bool _isEnabled;

#nullable disable

		[ObservableProperty]
		private AssistantScript _assistantScript;

        public AssistantActionEntity() { }

        public AssistantActionEntity(AssistantActionEntity entity)
        {
            this.Id = entity.Id;
			this._name = entity.Name;
			this._command = entity.Command;
			this._description = entity.Description;
			this._needsConfirmation = entity.NeedsConfirmation;
			this._isEnabled = entity.IsEnabled;
			this._assistantScript = entity.AssistantScript;
			this.AssistantScriptId = entity.AssistantScriptId;
        }
    }
}
