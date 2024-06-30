using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

		private string _name = string.Empty;

		[CustomValidation(typeof(AssistantActionEntity), nameof(ValidateNameCommand))]
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value, true);
		}

		private string _command = string.Empty;

		[CustomValidation(typeof(AssistantActionEntity), nameof(ValidateNameCommand))]
		public string Command
		{
			get => _command;
			set => SetProperty(ref _command, value, true);
		}

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

		public static ValidationResult ValidateNameCommand(string str, ValidationContext context)
		{
			if(string.IsNullOrWhiteSpace(str))
				return new ValidationResult($"{context.MemberName} can't be empty!");
			else
				return ValidationResult.Success;
		}
    }
}
