using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VoiceAssistant.Components
{
	/// <summary>
	/// Логика взаимодействия для AssistantActionFilter.xaml
	/// </summary>
	public partial class AssistantActionFilter : UserControl
	{
		#region Dependency properties

		#region ClearCommand

		public static readonly DependencyProperty ClearCommandProperty =
			DependencyProperty.Register("ClearCommand", typeof(ICommand),
				typeof(AssistantActionFilter), new PropertyMetadata(null, ClearCommandPropertyChanged));

		private static void ClearCommandPropertyChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var obj = d as AssistantActionFilter;
			if (obj is null)
				return;

			obj.ClearCommandPropertyChanged((ICommand)e.NewValue);
		}

		private void ClearCommandPropertyChanged(ICommand newValue)
		{
			ClearButton.Command = newValue;
		}

		public ICommand ClearCommand
		{
			get => (ICommand)GetValue(ClearCommandProperty);
			set => SetValue(ClearCommandProperty, value);
		}

		#endregion

		#region ApplyCommand

		public static readonly DependencyProperty ApplyCommandProperty =
			DependencyProperty.Register("ApplyCommand", typeof(ICommand),
				typeof(AssistantActionFilter), new PropertyMetadata(null, ApplyCommandPropertyChanged));

		private static void ApplyCommandPropertyChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var obj = d as AssistantActionFilter;
			if (obj is null)
				return;

			obj.ApplyCommandPropertyChanged((ICommand)e.NewValue);
		}

		private void ApplyCommandPropertyChanged(ICommand newValue)
		{
			ApplyButton.Command = newValue;
		}

		public ICommand ApplyCommand
		{
			get => (ICommand)GetValue(ApplyCommandProperty);
			set => SetValue(ApplyCommandProperty, value);
		}

		#endregion

		#endregion

		public AssistantActionFilter()
		{
			InitializeComponent();
		}
	}
}
