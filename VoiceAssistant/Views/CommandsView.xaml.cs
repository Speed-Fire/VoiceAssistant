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
using VoiceAssistant.Domain.Models;

namespace VoiceAssistant.Views
{
	/// <summary>
	/// Логика взаимодействия для CommandsView.xaml
	/// </summary>
	public partial class CommandsView : UserControl
	{
		public CommandsView()
		{
			InitializeComponent();

			var script = new AssistantScript()
			{
				Id = 1,
				Name = "Test script"
			};

			var action = new AssistantAction()
			{
				Command = "Open the fucking door",
				Name = "Test action",
				Description = "Sometimes i think i could reach much more than i have done now.",
				NeedsConfirmation = false,
				AssistantScript = script
			};

			comp.DataContext = action;
		}
	}
}
