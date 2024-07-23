using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VoiceAssistant.Views.AssistantCommands
{
	internal class StringToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch (value)
			{
				case string str:

					if (string.IsNullOrEmpty(str) ||
						(str.StartsWith('{') && str.EndsWith('}')))
						return null!;

					if (str.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
						return true;
					else if (str.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase))
						return false;
					else
						throw new Exception($"Can't cast \"{str}\" to bool!");

				case bool boolean:
					return boolean;

				default:
					return null!;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolean = (bool)value;

			if (targetType == typeof(bool))
				return boolean;
			else if (targetType == typeof(string))
				return boolean ? bool.TrueString : bool.FalseString;
			else
				throw new Exception("Unsupported target type!");
		}
	}
}
