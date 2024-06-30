using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Domain.Models;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.AssistantActionServices;
using VoiceAssistant.Services.Entities;
using VoiceAssistant.Views;
using VoiceAssistant.Views.AssistantActions;

namespace VoiceAssistant.ViewModels.AssistantActions
{
	internal partial class ChangeAssistantActionVM : ViewModel<ChangeAssistantActionView>
	{
        private readonly IAssistantActionService _assistantActionService;
        private readonly IUrgentNotifier _urgentNotifier;

        public bool IsUpdatingMode { get; }
        public AssistantActionEntity AssistantAction { get; }

		public ChangeAssistantActionVM(IAssistantActionService service,
            IUrgentNotifier urgentNotifier)
		{
			_assistantActionService = service;

			IsUpdatingMode = false;
			AssistantAction = new();

			AssistantAction.ErrorsChanged += AssistantAction_ErrorsChanged;
			_urgentNotifier = urgentNotifier;
		}

		public ChangeAssistantActionVM(IAssistantActionService service,
			IUrgentNotifier urgentNotifier,
			AssistantActionEntity action)
		{
			_assistantActionService = service;

			IsUpdatingMode = true;
			AssistantAction = new(action);

			AssistantAction.ErrorsChanged += AssistantAction_ErrorsChanged;
			_urgentNotifier = urgentNotifier;
		}

		[RelayCommand]
        private void Cancel()
        {
            Navigation.ReleaseDialog<AssistantActionEntity?>(false, null);
        }

        [RelayCommand(CanExecute = nameof(CanChangeCommandExecute))]
        private async Task Change()
        {
            bool res = false;

            if(IsUpdatingMode)
            {
                res = await _assistantActionService.UpdateAsync(AssistantAction);
            }
            else
            {
                res = await _assistantActionService.CreateAsync(AssistantAction);
            }

            if (!res)
            {
                // error handling
                var msg = IsUpdatingMode ? "Can't change this action." : "Can't create an action.";
                _urgentNotifier
                    .NotifyError(msg);

                return;
            }

			AssistantAction.ErrorsChanged -= AssistantAction_ErrorsChanged;
			Navigation.ReleaseDialog<AssistantActionEntity?>(true, AssistantAction);
        }

        private bool CanChangeCommandExecute()
        {
            return !AssistantAction.HasErrors;
        }

		private void AssistantAction_ErrorsChanged(object? sender,
			System.ComponentModel.DataErrorsChangedEventArgs e)
		{
            ChangeCommand.NotifyCanExecuteChanged();
		}
	}
}
