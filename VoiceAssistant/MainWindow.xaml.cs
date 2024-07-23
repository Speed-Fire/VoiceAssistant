using Synergy.WPF.Common.Controls;
using Synergy.WPF.Navigation.Components;
using Synergy.WPF.Navigation.Managers;
using Synergy.WPF.Navigation.Misc;
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
			NavigationManager navigationManager,
			UrgentNotificatorComponent urgentNotificator,
			VoiceAssistantListeningStatusComponent listeningComponent,
			MessageComponent messageComponent,
			MainVM vm)
		{
			InitializeComponent();

			navigationManager.Attach(NavConsts.MAIN_NAVIGATION_CHANNEL,
				AttachFrame, DetachFrame);

			this.MaxWidth = SystemParameters.WorkArea.Width;
			this.MaxHeight = SystemParameters.WorkArea.Height;

			DataContext = vm;

			SetupUrgentNotificator(urgentNotificator);
			SetupListeningComponent(listeningComponent);
			SetupMessageComponent(messageComponent);

			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var item = (NavItem)NavBar.Items[0];
			item.IsSelected = true;
		}

		#region Attach Frame

		private void AttachFrame(UserControlFrame frame)
		{
			AdornerDecorator.Child = frame;
		}

		private void DetachFrame(UserControlFrame frame)
		{
			AdornerDecorator.Child = null;
		}

		#endregion

		#region Setup components

		private void SetupMessageComponent(MessageComponent messageComponent)
		{
			Grid.SetColumn(messageComponent, 1);
			Grid.SetColumnSpan(messageComponent, 2);
			Grid.SetRowSpan(messageComponent, 4);

			Panel.SetZIndex(messageComponent, 10);

			MainGrid.Children.Add(messageComponent);
		}

		private void SetupUrgentNotificator(UrgentNotificatorComponent urgentNotificator)
		{
			urgentNotificator.SetValue(Grid.ColumnProperty, 2);
			urgentNotificator.SetValue(Grid.RowProperty, 2);

			MainGrid.Children.Add(urgentNotificator);
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