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

namespace VoiceAssistant
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly UrgentNotificatorComponent _urgentNotificator;

		public MainWindow(UserControlFrame frame,
			UrgentNotificatorComponent urgentNotificator)
		{
			InitializeComponent();

			this.MaxWidth = SystemParameters.WorkArea.Width;
			this.MaxHeight = SystemParameters.WorkArea.Height;

			_urgentNotificator = urgentNotificator;

			SetupFrame(frame);
			SetupUrgentNotificator(urgentNotificator);
		}

		private void SetupUrgentNotificator(UrgentNotificatorComponent urgentNotificator)
		{
			urgentNotificator.SetValue(Grid.ColumnProperty, 1);
			urgentNotificator.SetValue(Grid.RowProperty, 2);

			MainGrid.Children.Add(urgentNotificator);
		}

		private void SetupFrame(UserControlFrame frame)
		{
			frame.SetValue(Grid.ColumnSpanProperty, 2);
			frame.SetValue(Grid.RowSpanProperty, 4);

			MainGrid.Children.Add(frame);
		}
	}
}