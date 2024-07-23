using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VoiceAssistant.ViewModels.AssistantCommands;

namespace VoiceAssistant.Views.AssistantCommands
{
#nullable disable

	public class AssistantScriptParameterTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (container is not FrameworkElement element ||
				item is not AssistantScriptParameter vm)
				return null;

			var type = vm.IntendedType;

			if (type == typeof(TimeSpan))
			{
				return (DataTemplate)element.FindResource("Templates.Local.TimePicker");
			}
			else
			{
				var typeCode = Type.GetTypeCode(type);
				switch (typeCode)
				{
					case TypeCode.Boolean:
						return (DataTemplate)element.FindResource("Templates.Local.BoolPicker");

					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
						return (DataTemplate)element.FindResource("Templates.Local.IntegerPicker");

					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return (DataTemplate)element.FindResource("Templates.Local.DoublePicker");

					case TypeCode.DateTime:
						return (DataTemplate)element.FindResource("Templates.Local.DatePicker");

					case TypeCode.String:
					default:
						return (DataTemplate)element.FindResource("Templates.Local.StringPicker");
				}
			}
		}
	}
}
