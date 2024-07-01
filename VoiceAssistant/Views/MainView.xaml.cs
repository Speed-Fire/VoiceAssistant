using Microsoft.Extensions.DependencyInjection;
using Synergy.WPF.Common.Controls;
using Synergy.WPF.Navigation.Components;
using Synergy.WPF.Navigation.Misc;
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
using VoiceAssistant.Components;

namespace VoiceAssistant.Views
{
	/// <summary>
	/// Логика взаимодействия для MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		public MainView([FromKeyedServices(NavConsts.SCOPED_SERVICE)] UserControlFrame frame,
			VoiceAssistantListeningStatusComponent listeningComponent)
		{
			InitializeComponent();

			listeningComponent.Width = 40;
			listeningComponent.Height = 40;
			NavBar.BottomContent = listeningComponent;

			frame.SetValue(Grid.ColumnProperty, 1);
			MainGrid.Children.Add(frame);

			Loaded += MainView_Loaded;
		}

		private void MainView_Loaded(object sender, RoutedEventArgs e)
		{
			var item = (NavItem)NavBar.Items[0];
			item.IsSelected = true;
		}
	}
}
