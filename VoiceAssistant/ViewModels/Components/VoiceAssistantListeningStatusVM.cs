using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Notifications.Urgent;
using VoiceAssistant.Services.Misc.Interfaces;

namespace VoiceAssistant.ViewModels.Components
{
	public partial class VoiceAssistantListeningStatusVM(
		IVoiceAssistantMonitor voiceAssistantMonitor,
		IUrgentNotifier notifier)
		: ViewModel
	{
		private readonly IUrgentNotifier _notifier = notifier;
		public IVoiceAssistantMonitor VoiceAssistantMonitor { get; } = voiceAssistantMonitor;

		[RelayCommand]
		private void ChangeListeningStatus()
		{
			var newValue = !VoiceAssistantMonitor.IsListening;

			var result = VoiceAssistantMonitor.TrySetStatus(newValue);

			switch (result)
			{
				default:
				case VoiceAssistantLockStatus.Unlocked:
					break;

				case VoiceAssistantLockStatus.Locked:
					_notifier.NotifyError("Can't enable listening in this tab.");
					break;

				case VoiceAssistantLockStatus.Blocked:
					_notifier.NotifyError("Voice assistant listener is blocked. Try restart the application.");
					break;
			}
		}
	}
}
