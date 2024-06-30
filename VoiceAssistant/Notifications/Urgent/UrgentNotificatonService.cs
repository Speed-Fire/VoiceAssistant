using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Hosting;
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
    public sealed partial class UrgentNotificationService : 
		ObservableObject,
		IHostedService,
		IDisposable
	{
		public record UrgentNotification(string Message, MessageBoxImage Image, TimeSpan Duration,
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
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private DateTime? _notificationExpiration;

		private Task? _serviceRun;

		private async Task Do(CancellationToken token)
		{
			IsNotificationShown = false;

			while (!token.IsCancellationRequested)
			{
				if (_notificationExpiration is not null)
				{
					if (!IsNotificationShown ||
						DateTime.Now >= _notificationExpiration)
					{
						await HideNotification();
					}

					continue;
				}

				if (!_notifications.IsEmpty &&
					_notifications.TryDequeue(out var notification))
				{
					ShowNotification(notification);
				}
			}
		}

		#region Public methods

		public void PushNotification(UrgentNotification notification)
		{
			_notifications.Enqueue(notification);
		}

		#endregion

		#region IHostedService implementation

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_serviceRun = Task.Run(() => Do(_cancellationTokenSource.Token), cancellationToken);

			return Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			if(_serviceRun is null)
				return;

			_cancellationTokenSource.Cancel();

			await _serviceRun;
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

		#endregion
	}
}
