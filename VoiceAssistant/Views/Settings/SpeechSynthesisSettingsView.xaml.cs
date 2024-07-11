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
using VoiceAssistant.ViewModels.Settings;

namespace VoiceAssistant.Views.Settings
{
	/// <summary>
	/// Логика взаимодействия для SpeechSynthesisSettingsView.xaml
	/// </summary>
	public partial class SpeechSynthesisSettingsView : UserControl
	{
		private volatile bool _dragStarted;

		public SpeechSynthesisSettingsView()
		{
			InitializeComponent();

			Loaded += (sender, e) =>
			{
				var vm = DataContext as SpeechSynthesisSettingsVM;
				if (vm is null)
					return;

				VolumeSlider.Value = vm.Volume;
			};
		}

		private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			_dragStarted = true;
        }

		private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			var vm = DataContext as SpeechSynthesisSettingsVM;
			if (vm is null)
				return;

			vm.Volume = Convert.ToInt32(VolumeSlider.Value);
			_dragStarted = false;
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (_dragStarted)
				return;

			var vm = DataContext as SpeechSynthesisSettingsVM;
			if (vm is null)
				return;

			vm.Volume = Convert.ToInt32(VolumeSlider.Value);
			_dragStarted = false;
		}
    }
}
