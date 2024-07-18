using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.Entities
{
	public partial class AssistantCommandEntity : PublicValidator
	{
		public long Id { get; set; } = 0;
		public long? AssistantScriptId { get; set; } = 0;

		#region Name
		private string _name = string.Empty;

		[CustomValidation(typeof(AssistantCommandEntity), nameof(ValidateNameCommand))]
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value, true);
		}
		#endregion

		#region Command
		private string _command = string.Empty;

		[CustomValidation(typeof(AssistantCommandEntity), nameof(ValidateNameCommand))]
		public string Command
		{
			get => _command;
			set 
			{
				if (!SetProperty(ref _command, value, true))
					return;

				OnCommandChanged(value);
			}
		}
		#endregion

		#region Script
		private AssistantScript? _assistantScript;

		[Required]
		public AssistantScript? AssistantScript
		{
			get => _assistantScript;
			set
			{
				if (!SetProperty(ref _assistantScript, value, true))
					return;

				AssistantScriptId = value?.Id;
			}
		}
		#endregion

		[ObservableProperty]
		private string? _description;

		[ObservableProperty]
		private bool _needsConfirmation;

		[ObservableProperty]
		private bool _isEnabled;

		public ObservableCollection<string> CommandInputParameters { get; } = [];

		public string Input => throw new NotImplementedException();

		#region Ctors

		public AssistantCommandEntity() { }

        public AssistantCommandEntity(AssistantCommandEntity entity)
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

		#endregion

		private void OnCommandChanged(string value)
		{
			// find all parameters

			CommandInputParameters.Clear();

			var openBracketPos = -1;
			for(int i = 0; i < value.Length; i++)
			{
				if(value[i] == '{')
				{
					openBracketPos = i;
				}
				else if (value[i] == '}' && openBracketPos > 0)
				{
					var offset = openBracketPos + 1;
					CommandInputParameters.Add(value[offset..i]);

					openBracketPos = -1;
				}
			}
		}

		#region Validation methods

		public static ValidationResult ValidateNameCommand(string str, ValidationContext context)
		{
			if (string.IsNullOrWhiteSpace(str))
				return new ValidationResult($"{context.MemberName} can't be empty!");
			else
				return ValidationResult.Success!;
		}

		#endregion
	}
}
