using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace PluginsSystem.Extensions
{
	internal static class FrameworkElementExtensions
	{
		public static void SetBinding(
			this FrameworkElement element,
			DependencyProperty property,
			object source,
			string propertyName,
			BindingMode mode = BindingMode.TwoWay,
			UpdateSourceTrigger sourceTrigger = UpdateSourceTrigger.PropertyChanged,
			bool setTargetNullValue = false)
		{
			var binding = new Binding(propertyName)
			{
				Source = source,
				Mode = mode,
				UpdateSourceTrigger = sourceTrigger
			};

			if (setTargetNullValue)
				binding.TargetNullValue = "";

			element.SetBinding(property, binding);
		}
	}
}
