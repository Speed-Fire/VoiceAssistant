using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoiceAssistant.UI.Common.Messages;

namespace VoiceAssistant.UI.Common.Extensions
{
	public static class MessageServiceExtensions
	{
		#region ShowError

		public static Task<MessageBoxResult> ShowErrorAsync(
			this IMessageService service,
			string message)
		{
			return service.ShowAsync(message, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public static MessageBoxResult ShowError(
			this IMessageService service,
			string message)
		{
			return service.Show(message, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public static Task<MessageBoxResult> ShowErrorAsync(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.ShowAsync(message, button, MessageBoxImage.Error);
		}

		public static MessageBoxResult ShowError(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.Show(message, button, MessageBoxImage.Error);
		}

		#endregion

		#region ShowWarning

		public static Task<MessageBoxResult> ShowWarningAsync(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.ShowAsync(message, button, MessageBoxImage.Warning);
		}

		public static MessageBoxResult ShowWarning(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.Show(message, button, MessageBoxImage.Warning);
		}

		#endregion

		#region ShowInfo

		public static Task<MessageBoxResult> ShowInfoAsync(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.ShowAsync(message, button, MessageBoxImage.Information);
		}

		public static MessageBoxResult ShowInfo(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.Show(message, button, MessageBoxImage.Information);
		}

		#endregion

		#region ShowQuestion

		public static Task<MessageBoxResult> ShowQuestionAsync(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.ShowAsync(message, button, MessageBoxImage.Question);
		}

		public static MessageBoxResult ShowQuestion(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.Show(message, button, MessageBoxImage.Question);
		}

		#endregion

		#region Show

		public static Task<MessageBoxResult> ShowAsync(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.ShowAsync(message, button, MessageBoxImage.None);
		}

		public static MessageBoxResult Show(
			this IMessageService service,
			string message,
			MessageBoxButton button)
		{
			return service.Show(message, button, MessageBoxImage.None);
		}

		#endregion
	}
}
