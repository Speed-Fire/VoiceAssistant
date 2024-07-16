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
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.AssistantCommands;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantCommands;

namespace VoiceAssistant.ViewModels.AssistantCommands
{
	internal partial class ChangeAssistantCommandVM : ViewModel<ChangeAssistantCommandView>
	{
        private readonly IAssistantCommandService _assistantCommandService;
        private readonly IUrgentNotifier _urgentNotifier;

        public bool IsUpdatingMode { get; }
        public AssistantCommandEntity AssistantCommand { get; }

		public ChangeAssistantCommandVM(IAssistantCommandService service,
            IUrgentNotifier urgentNotifier)
		{
			_assistantCommandService = service;

			IsUpdatingMode = false;
			AssistantCommand = new();

			AssistantCommand.ErrorsChanged += AssistantCommand_ErrorsChanged;
			_urgentNotifier = urgentNotifier;
		}

		public ChangeAssistantCommandVM(IAssistantCommandService service,
			IUrgentNotifier urgentNotifier,
			AssistantCommandEntity command)
		{
			_assistantCommandService = service;

			IsUpdatingMode = true;
			AssistantCommand = new(command);

			AssistantCommand.ErrorsChanged += AssistantCommand_ErrorsChanged;
			_urgentNotifier = urgentNotifier;
		}

		[RelayCommand]
        private void Cancel()
        {
            Navigation.ReleaseDialog<AssistantCommandEntity?>(false, null);
        }

        [RelayCommand(CanExecute = nameof(CanChangeCommandExecute))]
        private async Task Change()
        {
            bool res = false;

            if(IsUpdatingMode)
            {
                res = await _assistantCommandService.UpdateAsync(AssistantCommand);
            }
            else
            {
                res = await _assistantCommandService.CreateAsync(AssistantCommand);
            }

            if (!res)
            {
                // error handling
                var msg = IsUpdatingMode ? "Can't change this command." : "Can't create a command.";
                _urgentNotifier
                    .NotifyError(msg);

                return;
            }

			AssistantCommand.ErrorsChanged -= AssistantCommand_ErrorsChanged;
			Navigation.ReleaseDialog<AssistantCommandEntity?>(true, AssistantCommand);
        }

        private bool CanChangeCommandExecute()
        {
            return !AssistantCommand.HasErrors;
        }

		private void AssistantCommand_ErrorsChanged(object? sender,
			System.ComponentModel.DataErrorsChangedEventArgs e)
		{
            ChangeCommand.NotifyCanExecuteChanged();
		}
	}
}
