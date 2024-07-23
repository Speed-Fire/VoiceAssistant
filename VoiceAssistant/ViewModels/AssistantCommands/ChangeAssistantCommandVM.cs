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
using VoiceAssistant.Entities;
using VoiceAssistant.Extensions;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.AssistantCommands;
using VoiceAssistant.Services.AssistantScripts;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantCommands;

namespace VoiceAssistant.ViewModels.AssistantCommands
{
	internal partial class ChangeAssistantCommandVM : ViewModel<ChangeAssistantCommandView>
	{
        private readonly IAssistantCommandService _assistantCommandService;
        private readonly IUrgentNotifier _urgentNotifier;

        public AssistantScriptParametersBinderVM ParametersBinder { get; }

        public bool IsUpdatingMode { get; }
        public AssistantCommandEntity AssistantCommand { get; }

		public ChangeAssistantCommandVM(
			IAssistantCommandService service,
			IUrgentNotifier urgentNotifier)
		{
			_assistantCommandService = service;

			IsUpdatingMode = false;
			AssistantCommand = new();

            ParametersBinder = new(AssistantCommand);

			ParametersBinder.ErrorsChanged += OnNestedVMsErrorsChanged;
			AssistantCommand.ErrorsChanged += OnNestedVMsErrorsChanged;

			_urgentNotifier = urgentNotifier;
		}

		public ChangeAssistantCommandVM(
            IAssistantCommandService service,
			IUrgentNotifier urgentNotifier,
			AssistantCommandEntity command)
		{
			_assistantCommandService = service;

			IsUpdatingMode = true;

			AssistantCommand = new(command);

			ParametersBinder = new(AssistantCommand);

			ParametersBinder.ErrorsChanged += OnNestedVMsErrorsChanged;
			AssistantCommand.ErrorsChanged += OnNestedVMsErrorsChanged;
			
			_urgentNotifier = urgentNotifier;
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
        private void Cancel()
        {
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
            return !AssistantCommand.HasErrors && !ParametersBinder.HasErrors;
        }

		#endregion

		#region Validation

		private void OnNestedVMsErrorsChanged(object? sender,
			System.ComponentModel.DataErrorsChangedEventArgs e)
		{
            ChangeCommand.NotifyCanExecuteChanged();
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
