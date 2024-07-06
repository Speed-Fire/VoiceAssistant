using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VoiceAssistant.Notifications.UrgentNotificationService;
using System.Windows;
using VoiceAssistant.Core.Interfaces;

namespace VoiceAssistant.Notifications.Urgent
{
	public class UrgentNotifier(UrgentNotificationService notificationService) : IUrgentNotifier
	{
		private readonly UrgentNotificationService _notificationService = notificationService;

		public void NotifyError(string message, int duration = 3000, Exception? exception = null)
		{
			var notification = new UrgentNotification(message, MessageBoxImage.Error,
				GetDuration(duration), exception?.Message);

			_notificationService.PushNotification(notification);
		}

		public void NotifyInfo(string message, int duration = 3000)
		{
			var notification = new UrgentNotification(message, MessageBoxImage.Information,
				GetDuration(duration));

			_notificationService.PushNotification(notification);
		}

		public void NotifyWarning(string message, int duration = 3000)
		{
			var notification = new UrgentNotification(message, MessageBoxImage.Warning,
				GetDuration(duration));

			_notificationService.PushNotification(notification);
		}

		private static TimeSpan GetDuration(int duration)
		{
			if (duration > 600)
				return TimeSpan.FromMilliseconds(duration);

			return TimeSpan.FromHours(1);
		}
	}
}
