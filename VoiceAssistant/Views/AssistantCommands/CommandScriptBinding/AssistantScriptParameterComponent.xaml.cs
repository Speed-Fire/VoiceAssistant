using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Synergy.WPF.Common.AttachedProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VoiceAssistant.ViewModels.AssistantCommands;

namespace VoiceAssistant.Views.AssistantCommands
{
	/// <summary>
	/// Логика взаимодействия для AssistantScriptParameterComponent.xaml
	/// </summary>
	public partial class AssistantScriptParameterComponent : UserControl
	{
		private AssistantScriptParameter? _parameter;

		public AssistantScriptParameterComponent()
		{
			InitializeComponent();

			DataContextChanged += OnDataContextChanged;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			if (_parameter is not null && _parameter.IntendedType == typeof(bool))
				_parameter.PropertyChanged -= VmPropertyChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_parameter is not null)
				_parameter.PropertyChanged -= VmPropertyChanged;

			if (e.NewValue is not AssistantScriptParameter parameter)
				return;

			_parameter = parameter;

			if (_parameter.IntendedType != typeof(bool))
				return;

			VmPropertyChanged(null, new("Value"));

			_parameter.PropertyChanged += VmPropertyChanged;
		}

		private void VmPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if(_parameter is not null &&
				e.PropertyName == nameof(AssistantScriptParameter.Value) &&
				_parameter.IntendedType == typeof(bool))
			{
				var value = _parameter.Value;

				if(value.Length > 2 && value[0] == '{' && value[^1] == '}')
					header_tb.Visibility = Visibility.Visible;
				else
					header_tb.Visibility = Visibility.Collapsed;
			}
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			if (_parameter is null)
				return;

			var data = (string)e.Data.GetData(DataFormats.Serializable);

			_parameter.Value = $"{{{data}}}";
		}

		private void AdvancedTextBox_PreviewDragOver(object sender, DragEventArgs e)
		{
			e.Handled = true;
		}
	}
}
