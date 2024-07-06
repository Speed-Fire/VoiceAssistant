using PluginsSystem.Models;
using PluginsSystem.Settings.ViewModels;
using Synergy.WPF.Common.AttachedProperties;
using Synergy.WPF.Common.Controls;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PluginsSystem.Extensions
{
	internal static class BindablePropertyExtensions
	{
		public static FrameworkElement[] CreateVisual(
			this BindableProperty property,
			Dictionary<Type, Style?> styles)
		{
			var typeCode = Type.GetTypeCode(property.ParameterInfo.ParameterType);

			switch (typeCode)
			{
				case TypeCode.String:
					return CreateTextBoxStringVisual(property, styles);

				case TypeCode.Decimal:
				case TypeCode.Int64:
				case TypeCode.Int32:
				case TypeCode.Int16:
				case TypeCode.SByte:
					return CreateTextBoxIntegerVisual(property, styles);

				case TypeCode.UInt64:
				case TypeCode.UInt32:
				case TypeCode.UInt16:
				case TypeCode.Byte:
					return CreateTextBoxUIntegerVisual(property, styles);

				case TypeCode.Single:
				case TypeCode.Double:
					return CreateTextBoxFloatVisual(property, styles);

				case TypeCode.Boolean:
					return CreateCheckBoxVisual(property, styles);

				case TypeCode.DateTime:
					return CreateDatePickerVisual(property, styles);

				default:
					throw new NotSupportedException("Parameter type is not supported!");
			}
		}

		#region TextBox visual

		public static FrameworkElement[] CreateTextBoxIntegerVisual(
			BindableProperty property,
			Dictionary<Type, Style?> styles)
		{
			return CreateTextBoxVisual(property, styles, "^[0-9-]*$", true);
		}

		public static FrameworkElement[] CreateTextBoxUIntegerVisual(
			BindableProperty property,
			Dictionary<Type, Style?> styles)
		{
			return CreateTextBoxVisual(property, styles, "^[0-9]*$", true);
		}

		public static FrameworkElement[] CreateTextBoxFloatVisual(
			BindableProperty property,
			Dictionary<Type, Style?> styles)
		{
			return CreateTextBoxVisual(property, styles, "^[0-9-.]*$", true);
		}

		public static FrameworkElement[] CreateTextBoxStringVisual(
			BindableProperty property,
			Dictionary<Type, Style?> styles)
		{
			return CreateTextBoxVisual(property, styles, null, false);
		}

		public static FrameworkElement[] CreateTextBoxVisual(
			BindableProperty property,
			Dictionary<Type, Style?> styles,
			string? regex,
			bool setTargetNullValue)
		{
			var parameterInfo = property.ParameterInfo;

			var result = new FrameworkElement[2];

			var textBlock = new TextBlock()
			{
				Style = styles[typeof(TextBlock)],
				Text = parameterInfo.Title,
				FontWeight = FontWeights.Bold
			};

			textBlock.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 5));

			var textBox = new AdvancedTextBox()
			{
				Style = styles[typeof(AdvancedTextBox)],
				HorizontalAlignment = HorizontalAlignment.Left,
				Width = 300
			};

			textBox.SetBinding(TextBox.TextProperty, property, "Value",
				setTargetNullValue: setTargetNullValue);

			if (!string.IsNullOrEmpty(regex))
			{
				textBox.SetValue(RegexAttachedProperty.ValueProperty, regex);
			}

			result[0] = textBlock;
			result[1] = textBox;

			return result;
		}

		#endregion

		#region CheckBox Visual

		public static FrameworkElement[] CreateCheckBoxVisual(
			BindableProperty property,
			Dictionary<Type, Style?> styles)
		{
			var parameterInfo = property.ParameterInfo;

			var result = new FrameworkElement[1];

			var checkBox = new CheckBox()
			{
				Style = styles[typeof(CheckBox)]
			};

			checkBox.SetBinding(
				CheckBox.IsCheckedProperty,
				property, "Value",
				setTargetNullValue: true);

			var textBlock = new TextBlock()
			{
				Style = styles[typeof(TextBlock)],
				Text = parameterInfo.Title,
				FontWeight = FontWeights.Bold
			};

			checkBox.Content = textBlock;

			result[0] = checkBox;

			return result;
		}

		#endregion

		#region DatePicker Visual

		public static FrameworkElement[] CreateDatePickerVisual(
			BindableProperty property,
			Dictionary<Type, Style?> styles)
		{
			var parameterInfo = property.ParameterInfo;

			var result = new FrameworkElement[2];

			var textBlock = new TextBlock()
			{
				Style = styles[typeof(TextBlock)],
				Text = parameterInfo.Title,
				FontWeight = FontWeights.Bold
			};

			textBlock.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 5));

			var datePicker = new DatePicker()
			{
				Style = styles[typeof(DatePicker)]
			};

			datePicker.SetBinding(
				DatePicker.SelectedDateProperty,
				property,
				"Value",
				setTargetNullValue: true);

			result[0] = textBlock;
			result[1] = datePicker;

			return result;
		}

		#endregion
	}
}
