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
using VoiceAssistant.ViewModels.Components;

namespace VoiceAssistant.Components
{
	/// <summary>
	/// Логика взаимодействия для VoiceAssistantListeningStatusComponent.xaml
	/// </summary>
	public partial class VoiceAssistantListeningStatusComponent : UserControl
	{
		public VoiceAssistantListeningStatusComponent(VoiceAssistantListeningStatusVM vm)
		{
			InitializeComponent();

			DataContext = vm;
		}
	}
}
