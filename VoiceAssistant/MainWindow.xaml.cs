using Synergy.WPF.Common.Controls;
using Synergy.WPF.Navigation.Components;
using System.Text;
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
using VoiceAssistant.Notifications;
using VoiceAssistant.ViewModels;

namespace VoiceAssistant
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(
			UserControlFrame frame,
			UrgentNotificatorComponent urgentNotificator,
			VoiceAssistantListeningStatusComponent listeningComponent,
			MainVM vm)
		{
			InitializeComponent();

			this.MaxWidth = SystemParameters.WorkArea.Width;
			this.MaxHeight = SystemParameters.WorkArea.Height;

			DataContext = vm;

			SetupFrame(frame);
			SetupUrgentNotificator(urgentNotificator);
			SetupListeningComponent(listeningComponent);

			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var item = (NavItem)NavBar.Items[0];
			item.IsSelected = true;
		}

		#region Setup components

		private void SetupUrgentNotificator(UrgentNotificatorComponent urgentNotificator)
		{
			urgentNotificator.SetValue(Grid.ColumnProperty, 2);
			urgentNotificator.SetValue(Grid.RowProperty, 2);

			MainGrid.Children.Add(urgentNotificator);
		}

		private void SetupFrame(UserControlFrame frame)
		{
			frame.SetValue(Grid.ColumnProperty, 1);
			frame.SetValue(Grid.ColumnSpanProperty, 2);
			frame.SetValue(Grid.RowSpanProperty, 4);

			MainGrid.Children.Add(frame);
		}

		private void SetupListeningComponent(VoiceAssistantListeningStatusComponent listeningComponent)
		{
			listeningComponent.Width = 40;
			listeningComponent.Height = 40;
			NavBar.BottomContent = listeningComponent;
		}

		#endregion
	}
}