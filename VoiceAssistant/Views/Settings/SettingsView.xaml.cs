using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Navigation.Components;
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
using VoiceAssistant.UI.Common.AttachedProperties;

namespace VoiceAssistant.Views.Settings
{
	/// <summary>
	/// Логика взаимодействия для SettingsView.xaml
	/// </summary>
	public partial class SettingsView : UserControl
	{
		public SettingsView(
			[FromKeyedServices(Synergy.WPF.Navigation.Misc.NavConsts.SCOPED_SERVICE)]
				UserControlFrame frame)
		{
			InitializeComponent();

			SetupFrame(frame);

			Loaded += (sender, e) => { SettingsLB.SelectedIndex = 0; };
			Unloaded += (sender, e) => { InnerGrid.Children.Clear(); };
		}

		private void SetupFrame(UserControlFrame frame)
		{
			frame.SetValue(Grid.ColumnProperty, 2);

			InnerGrid.Children.Add(frame);
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			if (e.AddedItems[0] is not FrameworkElement element)
				return;

			if (element.GetValue(CommandAttachedProperty.ValueProperty) is not ICommand command)
				return;

			command.Execute(null);
		}
    }
}
