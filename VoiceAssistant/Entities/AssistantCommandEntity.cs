using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.Entities
{
	public partial class AssistantCommandEntity : PublicValidator
	{
		public long Id { get; set; } = 0;
		public long? AssistantScriptId { get; set; } = 0;

		#region Name
		private string _name = string.Empty;

		[CustomValidation(typeof(AssistantCommandEntity), nameof(ValidateNotEmpty))]
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value, true);
		}
		#endregion

		#region Command
		private string _command = string.Empty;

		[CustomValidation(typeof(AssistantCommandEntity), nameof(ValidateNotEmpty))]
		[CustomValidation(typeof(AssistantCommandEntity), nameof(ValidateCommand))]
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
		private AssistantScript? _script;

		[CustomValidation(typeof(AssistantCommandEntity), nameof(ValidateNotEmpty))]
		public AssistantScript? Script
		{
			get => _script;
			set
			{
				if (!SetProperty(ref _script, value, true))
					return;

				Input = [];
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

		public List<string> Input { get; set; } = [];

		#region Ctors

		public AssistantCommandEntity()
		{
			CommandInputParameters.CollectionChanged += OnInputCollectionChanged;
		}

        public AssistantCommandEntity(AssistantCommandEntity entity)
        {
            this.Id = entity.Id;
			this._name = entity.Name;
			this._command = entity.Command;
			this._description = entity.Description;
			this._needsConfirmation = entity.NeedsConfirmation;
			this._isEnabled = entity.IsEnabled;
			this._script = entity.Script;
			this.AssistantScriptId = entity.AssistantScriptId;
			
			CommandInputParameters.CollectionChanged += OnInputCollectionChanged;
        }

		#endregion

		#region Property changed

		private void OnCommandChanged(string value)
		{
			if (GetErrors(nameof(Command)).Any())
				return;

			// find all parameters		
			List<string> newCurrent = FindAllParameters(value);

			// remove not found items
			var toRemove = CommandInputParameters.Except(newCurrent).ToList();
			foreach (var item in toRemove)
				CommandInputParameters.Remove(item);

			// add/insert new items
			for (int i = 0; i < newCurrent.Count; i++)
			{
				if (i < CommandInputParameters.Count)
				{
					if (newCurrent[i] == CommandInputParameters[i])
						continue;
					else
						CommandInputParameters.Insert(i, newCurrent[i]);
				}
				else
				{
					CommandInputParameters.Add(newCurrent[i]);
				}
			}
		}

		private void OnInputCollectionChanged(object? sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			ValidateProperty(null, nameof(CommandInputParameters));
		}

		#endregion

		#region Internal

		private static List<string> FindAllParameters(string value)
		{
			var newCurrent = new List<string>();
			var openBracketPos = -1;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == '{')
				{
					openBracketPos = i;
				}
				else if (value[i] == '}' && openBracketPos > 0)
				{
					var offset = openBracketPos + 1;
					newCurrent.Add(value[offset..i]);

					openBracketPos = -1;
				}
			}

			return newCurrent;
		}

		#endregion

		#region Validation methods

		public static ValidationResult ValidateNotEmpty(object obj, ValidationContext context)
		{
			if ((obj is string str && string.IsNullOrWhiteSpace(str)) ||
				obj is null)
			{
				var errorTemplate = 
					GetResource<string>("Strings.AssistantCommand.Change.Validation.Empty");

				return new(string.Format(errorTemplate, context.DisplayName));
			}

			return ValidationResult.Success!;
		}

		public static ValidationResult ValidateCommand(string str, ValidationContext context)
		{
			if (!string.IsNullOrWhiteSpace(str))
			{
				var parameters = FindAllParameters(str);

				if (parameters.Count != parameters.Distinct().Count())
				{
					var error =
						GetResource<string>("Strings.AssistantCommand.Change.Validation.Command.Duplicates");

					return new(error);
				}

				if (parameters.Any(s => s.Length == 0))
				{
					var error =
						GetResource<string>("Strings.AssistantCommand.Change.Validation.Command.EmptyInputParameter");

					return new(error);
				}
			}

			return ValidationResult.Success!;
		}

		#endregion
	}
}
