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
using VoiceAssistant.Helpers;

namespace VoiceAssistant.Components
{
	/// <summary>
	/// Логика взаимодействия для WindowUpperPanel.xaml
	/// </summary>
	public partial class WindowUpperPanel : UserControl
	{
		private readonly Window _window = Application.Current.MainWindow;

		public WindowUpperPanel()
		{
			InitializeComponent();
		}

		#region Window buttons

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			_window.Close();
        }

		private void ExpandButton_Click(object sender, RoutedEventArgs e)
		{
			WindowHelper.DoMaximize();
		}

		private void HideButton_Click(object sender, RoutedEventArgs e)
		{
			_window.WindowState = WindowState.Minimized;
		}

		#endregion

		#region DragMove

		private readonly object _lock = new();

		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (WindowHelper.IsMaximized)
				{
					lock (_lock)
					{
						if (WindowHelper.IsMaximized)
							WindowHelper.NormilizeAndAdjustToMouse();
					}
				}
				
				_window.DragMove();
			}
		}

		#endregion
	}
}
