using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoiceAssistant.Notifications.Urgent;

namespace VoiceAssistant.Notifications
{
    public sealed partial class UrgentNotificationHandler : 
		ObservableObject,
		IUrgentNotificator,
		IDisposable
	{
		private record UrgentNotification(string Message, MessageBoxImage Image, TimeSpan Duration,
			string? AdditionalInfo = null);

		[ObservableProperty]
		private bool _isNotificationShown;

		[ObservableProperty]
		private string? _message;

		[ObservableProperty]
		private MessageBoxImage? _image;

		[ObservableProperty]
		private string? _additionalInfo;

		private readonly ConcurrentQueue<UrgentNotification> _notifications = [];
		private CancellationTokenSource? _cancellationTokenSource;
		private DateTime? _notificationExpiration;

		private volatile bool _isRunning = false;

		private async void Start(CancellationToken? token = null)
		{
			IsNotificationShown = false;

			if(token is null)
			{
				_cancellationTokenSource?.Dispose();
				_cancellationTokenSource = new();

				token = _cancellationTokenSource.Token;
			}

			while (!token.Value.IsCancellationRequested)
			{
				if(_notificationExpiration is not null)
				{
					if(!IsNotificationShown || 
						DateTime.Now >= _notificationExpiration)
					{
						await HideNotification();
					}

					continue;
				}

				if(!_notifications.IsEmpty &&
					_notifications.TryDequeue(out var notification))
				{
					ShowNotification(notification);
				}
			}

			_isRunning = false;
		}

		private readonly object _lock = new();

		public void StartAsync(CancellationToken? token = null)
		{
			if (!_isRunning)
			{
				lock(_lock)
				{
					if (!_isRunning)
					{
						Task.Run(() => Start(token));

						_isRunning = true;
					}
				}
			}
		}

		public void Stop()
		{
			if(_cancellationTokenSource is null)
			{
				throw new InvalidOperationException("Urgent notificator was started with external CancellationToken!");
			}

			_cancellationTokenSource.Cancel();
		}

		#region IUrgentNotifier implementation

		public void NotifyError(string message, int duration = 3000, Exception? exception = null)
		{
			var notification = new UrgentNotification(message, MessageBoxImage.Error,
				GetDuration(duration), exception?.Message);

			_notifications.Enqueue(notification);
		}

		public void NotifyInfo(string message, int duration = 3000)
		{
			var notification = new UrgentNotification(message, MessageBoxImage.Information,
				GetDuration(duration));

			_notifications.Enqueue(notification);
		}

		public void NotifyWarning(string message, int duration = 3000)
		{
			var notification = new UrgentNotification(message, MessageBoxImage.Warning,
				GetDuration(duration));

			_notifications.Enqueue(notification);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose()
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
		}

		#endregion

		#region Internal

		private void ShowNotification(UrgentNotification notification)
		{
			Message = notification.Message;
			Image = notification.Image;
			AdditionalInfo = notification.AdditionalInfo;

			_notificationExpiration = DateTime.Now + notification.Duration;

			IsNotificationShown = true;
		}

		private async Task HideNotification()
		{
			IsNotificationShown = false;

			await Task.Delay(500);

			Message = null;
			Image = null;
			AdditionalInfo = null;

			_notificationExpiration = null;
		}

		private static TimeSpan GetDuration(int duration)
		{
			if(duration > 600)
				return TimeSpan.FromMilliseconds(duration);

			return TimeSpan.FromHours(1);
		}

		#endregion
	}
}
