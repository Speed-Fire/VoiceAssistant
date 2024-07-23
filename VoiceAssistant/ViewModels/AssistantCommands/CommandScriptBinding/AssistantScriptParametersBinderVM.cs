using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Synergy.Core.Collections.Observable;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.ViewModels.AssistantCommands
{
	public partial class AssistantScriptParametersBinderVM : ViewModel
    {
		private readonly AssistantCommandEntity _entity;

		[ObservableProperty]
		private bool _hasUnusedCommandInputs = false;

		[CustomValidation(typeof(AssistantScriptParametersBinderVM), nameof(ValidateParameters))]
        public FullyObservableCollection<AssistantScriptParameter> Parameters { get; } = [];

		public IEnumerable<string> EntityInputs => _entity.CommandInputParameters;

        public AssistantScriptParametersBinderVM(AssistantCommandEntity entity)
        {
            _entity = entity;

			GenerateParameters(entity.Script);
			FillParameters(entity.Script);

			_entity.PropertyChanged += Entity_PropertyChanged;
			_entity.CommandInputParameters.CollectionChanged +=
				CommandInputParameters_CollectionChanged;

			Parameters.ItemPropertyChanged += OnParameterItemPropertyChanged;
        }

		#region Event handlers

		private void CommandInputParameters_CollectionChanged(object? sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{

				foreach (string input in e.OldItems!)
				{
					var parameter = Parameters
						.FirstOrDefault(p => p.Value == $"{{{input}}}");

					if (parameter is null)
						continue;

					parameter.ClearCommand.Execute(null);
				}

				UpdateEntityInput();
			}

			RecalculateUnusedCommandInputs();
		}

		private void Entity_PropertyChanged(object? sender,
			System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(AssistantCommandEntity.Script):
					GenerateParameters(_entity.Script);
					UpdateEntityInput();
					RecalculateUnusedCommandInputs();
					break;
				default:
					break;
			}
		}

		private void OnParameterItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)
		{
			ValidateProperty(null, nameof(Parameters));

			UpdateEntityInput();
			RecalculateUnusedCommandInputs();
		}

		#endregion

		#region Validation

		public static ValidationResult ValidateParameters(IEnumerable<AssistantScriptParameter> _,
			ValidationContext context)
		{
			var collection = ((AssistantScriptParametersBinderVM)context.ObjectInstance).Parameters;

			if (collection.Where(item => item.HasErrors).Any())
			{
				var error =
					GetAppResource<string>("Strings.AssistantCommand.Change.Validation.Script.IncorrectParameter");

				return new(error);
			}

			return ValidationResult.Success!;
		}

		#endregion

		#region Internal

		private void RecalculateUnusedCommandInputs()
		{
			var bindedValues = Parameters
				.Where(parameter => parameter.Value.Length >= 2 &&
					parameter.Value[0] == '{' && parameter.Value[^1] == '}')
				.Select(parameter => parameter.Value[1..^1]);

			foreach(var commandInput in _entity.CommandInputParameters)
			{
				if (!bindedValues.Contains(commandInput))
				{
					HasUnusedCommandInputs = true;
					return;
				}
			}

			HasUnusedCommandInputs = false;
		}

		private void UpdateEntityInput()
		{
			var inputs = Parameters.Select(p => p.Value);

			_entity.Input = inputs.ToList();
		}

		private void GenerateParameters(AssistantScript? script)
		{
			Parameters.Clear();
			if (script is null)
				return;

			foreach(var scriptParameter in script.Parameters)
			{
				var (paramName, paramType) = scriptParameter.ToValueTuple();

				var parameter = new AssistantScriptParameter(paramName, paramType, this);

				Parameters.Add(parameter);
			}
		}

		private void FillParameters(AssistantScript? script)
		{
			if (script is null)
				return;

			if (_entity.Input.Count != script.Parameters.Count)
				throw new Exception("Incorrect count of input parameters!");

			var i = 0;
			foreach(var parameter in Parameters)
			{
				parameter.Value = _entity.Input[i++];
			}
		}

		#endregion

		#region Dispose

		public override void Dispose()
		{
			base.Dispose();

			_entity.PropertyChanged -= Entity_PropertyChanged;
			_entity.CommandInputParameters.CollectionChanged -=
				CommandInputParameters_CollectionChanged;
		}

		#endregion
	}
}
