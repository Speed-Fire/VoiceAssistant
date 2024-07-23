using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoiceAssistant.UI.Common.Messages;

namespace VoiceAssistant.ViewModels
{
	internal partial class MessageService : ObservableObject, IMessageService
	{
		private readonly object _lock = new();

		[ObservableProperty]
		private string _message = string.Empty;

		[ObservableProperty]
		private MessageBoxButton _button;

		[ObservableProperty]
		private MessageBoxImage _image;

		[ObservableProperty]
		private bool _isVisible = false;

		private MessageBoxResult? Result { get; set; }

		public MessageBoxResult Show(string message, MessageBoxButton button, MessageBoxImage image)
		{
			lock (_lock)
			{
				Result = null;
				Button = button;
				Image = image;
				Message = message;

				IsVisible = true;
				while (Result is null) { }

				IsVisible = false;
				Thread.Sleep(300);

				return Result.Value;
			}
		}

		public Task<MessageBoxResult> ShowAsync(string message, MessageBoxButton button, MessageBoxImage image)
		{
			return Task.Run(() => Show(message, button, image));
		}

		[RelayCommand]
		private void SetResult(MessageBoxResult result)
		{
			Result = result;
		}
	}
}
