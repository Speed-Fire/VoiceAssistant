using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceAssistant.UI.Common.Adorners;
using VoiceAssistant.UI.Common.Helpers;

namespace VoiceAssistant.Views.AssistantCommands
{
	/// <summary>
	/// Логика взаимодействия для ChangeAssistantActionView.xaml
	/// </summary>
	public partial class ChangeAssistantCommandView : UserControl
	{
		private DraggableAdorner? _draggableAdorner;

		public ChangeAssistantCommandView()
		{
			InitializeComponent();

			PreviewGiveFeedback += OnParameterPreviewGiveFeedback;
		}

		private void InputParameterMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed &&
				sender is FrameworkElement element &&
				element.DataContext is not null)
			{
				var data = new DataObject(DataFormats.Serializable, element.DataContext);

				var currentLayer = AdornerLayer.GetAdornerLayer(this);
				_draggableAdorner = new(element, e.GetPosition(element));

				currentLayer.Add(_draggableAdorner);
				DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
				currentLayer.Remove(_draggableAdorner);
			}
		}

		private void OnParameterPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			_draggableAdorner!.MoveToMousePosition();
		}

		private void OnValidationError(object sender, ValidationErrorEventArgs e)
		{
			if (e.Action == ValidationErrorEventAction.Added)
				ErrorsListBox.Items.Add(e.Error.ErrorContent);
			else
				ErrorsListBox.Items.Remove(e.Error.ErrorContent);
		}
	}
}
