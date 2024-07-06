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
	/// Логика взаимодействия для PluginComponent.xaml
	/// </summary>
	public partial class PluginComponent : UserControl
	{
		#region Settings Command

		public static readonly DependencyProperty SettingsCommandProperty =
			DependencyProperty.Register("SettingsCommand", typeof(ICommand), typeof(PluginComponent),
				new PropertyMetadata(null, SettingsCommandChanged));

		private static void SettingsCommandChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			if (d is not PluginComponent component)
				return;

			component.SettingsButton.Command = e.NewValue as ICommand;

		}

		public ICommand SettingsCommand
		{
			get => (ICommand)GetValue(SettingsCommandProperty);
			set => SetValue(SettingsCommandProperty, value);
		}

		#endregion

		public PluginComponent()
		{
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			System.Diagnostics.Process.Start("explorer", e.Uri.ToString());
        }
    }
}
