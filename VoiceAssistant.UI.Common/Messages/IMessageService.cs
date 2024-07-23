using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoiceAssistant.UI.Common.Messages
{
	public interface IMessageService
	{
		MessageBoxResult Show(string message, MessageBoxButton button, MessageBoxImage image);
		Task<MessageBoxResult> ShowAsync(string message, MessageBoxButton button, MessageBoxImage image);
	}
}
