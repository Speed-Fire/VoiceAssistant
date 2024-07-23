using System;
using System.Collections.Generic;
using System.Globalization;
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
using VoiceAssistant.UI.Common.Messages;

namespace VoiceAssistant.Components
{
	namespace MessageBox
	{
		public class MessageBoxImageConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (value is not MessageBoxImage image)
					throw new ArgumentException("", nameof(value));

				return image switch
				{
					MessageBoxImage.Error => Application.Current.FindResource("Bitmaps.Error64"),
					MessageBoxImage.Warning => Application.Current.FindResource("Bitmaps.Attention64"),
					MessageBoxImage.Question => Application.Current.FindResource("Bitmaps.Question64"),
					MessageBoxImage.Information => Application.Current.FindResource("Bitmaps.Info64"),
					_ => null!,
				};
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		public class ButtonDataTemplateSelector : DataTemplateSelector
		{
			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				if (item is null)
					return null!;

				if(item is not MessageBoxButton button)
					throw new ArgumentException("", nameof(item));

				if(container is not FrameworkElement element)
					throw new ArgumentException("", nameof(container));

				return button switch
				{
					MessageBoxButton.OKCancel => (DataTemplate)element.FindResource("Templates.Local.Buttons.OKCancel"),
					MessageBoxButton.YesNoCancel => (DataTemplate)element.FindResource("Templates.Local.Buttons.YesNoCancel"),
					MessageBoxButton.YesNo => (DataTemplate)element.FindResource("Templates.Local.Buttons.YesNo"),
					_ => (DataTemplate)element.FindResource("Templates.Local.Buttons.OK"),
				};
			}
		}
	}

	/// <summary>
	/// Логика взаимодействия для MessageComponent.xaml
	/// </summary>
	public partial class MessageComponent : UserControl
	{
		public MessageComponent(IMessageService service)
		{
			InitializeComponent();

			DataContext = service;
		}
	}
}
