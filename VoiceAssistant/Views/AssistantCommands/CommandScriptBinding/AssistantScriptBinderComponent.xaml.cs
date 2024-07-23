using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace VoiceAssistant.Views.AssistantCommands
{
	/// <summary>
	/// Логика взаимодействия для AssistantScriptBinderComponent.xaml
	/// </summary>
	public partial class AssistantScriptBinderComponent : UserControl
	{
		#region ItemsSource

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
				typeof(AssistantScriptBinderComponent),
				new PropertyMetadata(null, ItemsSourceChanged));

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		private static void ItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not AssistantScriptBinderComponent binder)
				return;

			binder.ItemListBox.ItemsSource = (IEnumerable)e.NewValue;
		}

		#endregion

		public AssistantScriptBinderComponent()
		{
			InitializeComponent();
		}
	}
}
