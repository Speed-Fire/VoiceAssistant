using Synergy.WPF.Common.AttachedProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VoiceAssistant.AttachedProperties
{
	internal class CommandAttachedProperty : BaseAttachedProperty<CommandAttachedProperty, ICommand>
	{
		private RoutedEventHandler? _unloaded;
		private MouseButtonEventHandler? _click;

		public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not FrameworkElement element)
				return;

			if(_unloaded is not null && _click is not null)
			{
				element.Unloaded -= _unloaded;
				element.MouseDown -= _click;
			}

			if (e.NewValue is not ICommand command)
				return;

			_click = (sender, e) =>
			{
				command.Execute(null);
			};

			_unloaded = (sender, e) =>
			{
				element.Unloaded -= _unloaded;
				element.MouseDown -= _click;
			};

			element.MouseDown += _click;
			element.Unloaded += _unloaded;
		}
	}
}
