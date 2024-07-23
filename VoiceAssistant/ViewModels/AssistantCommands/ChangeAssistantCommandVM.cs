using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Extensions;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.AssistantCommands;
using VoiceAssistant.Services.AssistantScripts;
using VoiceAssistant.UI.Common.Messages;
using VoiceAssistant.Views;
using VoiceAssistant.UI.Common.Extensions;
using VoiceAssistant.Views.AssistantCommands;
using System.ComponentModel;

namespace VoiceAssistant.ViewModels.AssistantCommands
{
	internal partial class ChangeAssistantCommandVM : ViewModel<ChangeAssistantCommandView>
	{
        private readonly IAssistantCommandService _assistantCommandService;
        private readonly IUrgentNotifier _urgentNotifier;
		private readonly IMessageService _messageService;

        public AssistantScriptParametersBinderVM ParametersBinder { get; }

        public bool IsUpdatingMode { get; }
        public AssistantCommandEntity AssistantCommand { get; }

		private bool Changed { get; set; } = false;

		public ChangeAssistantCommandVM(
			IAssistantCommandService service,
			IUrgentNotifier urgentNotifier,
			IMessageService messageService)
		{
			_assistantCommandService = service;
			_urgentNotifier = urgentNotifier;
			_messageService = messageService;

			IsUpdatingMode = false;
			AssistantCommand = new();

            ParametersBinder = new(AssistantCommand);

			ParametersBinder.ErrorsChanged += OnNestedVMsErrorsChanged;
			AssistantCommand.ErrorsChanged += OnNestedVMsErrorsChanged;

			ParametersBinder.PropertyChanged += NestedVMPropertyChanged;
			AssistantCommand.PropertyChanged += NestedVMPropertyChanged;
		}

		public ChangeAssistantCommandVM(
            IAssistantCommandService service,
			IUrgentNotifier urgentNotifier,
			IMessageService messageService,
			AssistantCommandEntity command)
		{
			_assistantCommandService = service;
			_urgentNotifier = urgentNotifier;
			_messageService = messageService;

			IsUpdatingMode = true;
			AssistantCommand = new(command);

			ParametersBinder = new(AssistantCommand);

			ParametersBinder.ErrorsChanged += OnNestedVMsErrorsChanged;
			AssistantCommand.ErrorsChanged += OnNestedVMsErrorsChanged;

			ParametersBinder.PropertyChanged += NestedVMPropertyChanged;
			AssistantCommand.PropertyChanged += NestedVMPropertyChanged;
		}

		#region Commands

		[RelayCommand]
		private void OpenScriptSelector()
		{
			Navigation.PushDialog<AssistantScriptSelectorVM, AssistantScript>(result =>
			{
				var dialogResult = result.Result;
				var script = result.ReturnValue;

				if (dialogResult != true || script is null)
					return;

				AssistantCommand.Script = script;
			});
		}

		[RelayCommand]
        private async Task Cancel()
        {
			if (Changed)
			{
				var message = GetAppResource<string>("Strings.Warnings.Unsaved");

				var result = await _messageService
					.ShowQuestionAsync(message, System.Windows.MessageBoxButton.YesNo);

				if (result != System.Windows.MessageBoxResult.Yes)
					return;
			}

            Navigation.ReleaseDialog<AssistantCommandEntity?>(false, null);
        }

        [RelayCommand(CanExecute = nameof(CanChangeCommandExecute))]
        private async Task Change()
        {
			AssistantCommand.ValidateAll();
			ParametersBinder.Validate();

			if (AssistantCommand.HasErrors || ParametersBinder.HasErrors)
			{
				var message = GetAppResource<string>("Strings.Validation.Field.Empty");

				_urgentNotifier.NotifyWarning(message);
				return;
			}

			if (ParametersBinder.HasUnusedCommandInputs)
			{
				var message = GetAppResource<string>("Strings.AssistantCommand.Change.InputParameters.Warning.Unused");
				var result = await _messageService.ShowWarningAsync(message,
					System.Windows.MessageBoxButton.YesNo);

				if (result != System.Windows.MessageBoxResult.Yes)
					return;
			}

            bool res = false;
            var command = AssistantCommand.Map();

            if(IsUpdatingMode)
            {
                res = await _assistantCommandService.UpdateAsync(command);
            }
            else
            {
                res = await _assistantCommandService.CreateAsync(command);
            }

            if (!res)
            {
                // error handling
                var msg = IsUpdatingMode ? "Can't change this command." : "Can't create a command.";
                _urgentNotifier
                    .NotifyError(msg);

                return;
            }

			AssistantCommand.ErrorsChanged -= OnNestedVMsErrorsChanged;
			Navigation.ReleaseDialog<AssistantCommandEntity?>(true, AssistantCommand);
        }

        private bool CanChangeCommandExecute()
        {
            return Changed && !AssistantCommand.HasErrors && !ParametersBinder.HasErrors;
        }

		#endregion

		#region Validation

		private void OnNestedVMsErrorsChanged(object? sender,
			System.ComponentModel.DataErrorsChangedEventArgs e)
		{
            ChangeCommand.NotifyCanExecuteChanged();
		}

		#endregion

		#region Property changed

		private void NestedVMPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			Changed = true;
			ChangeCommand.NotifyCanExecuteChanged();

			ParametersBinder.PropertyChanged -= NestedVMPropertyChanged;
			AssistantCommand.PropertyChanged -= NestedVMPropertyChanged;
		}

		#endregion

		#region Dispose

		public override void Dispose()
		{
			ParametersBinder.Dispose();
		}

		#endregion
	}
}
