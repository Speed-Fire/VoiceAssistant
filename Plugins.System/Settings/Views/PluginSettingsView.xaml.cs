using PluginsSystem.Entities;
using PluginsSystem.Extensions;
using PluginsSystem.Models;
using PluginsSystem.Settings.ViewModels;
using Synergy.WPF.Common.AttachedProperties;
using Synergy.WPF.Common.Controls;
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

namespace PluginsSystem.Settings.Views
{
	/// <summary>
	/// Логика взаимодействия для PluginSettingsView.xaml
	/// </summary>
	public partial class PluginSettingsView : UserControl
	{
		private readonly Dictionary<Type, Style?> _styles;

		private readonly Style? _textBlockStyle    = GetResource<Style>("Styles.TextBlock");
		private readonly Style? _normalButtonStyle = GetResource<Style>("Styles.NormalButton");
		private readonly Style? _delimeterStyle    = GetResource<Style>("Styles.Delimeter.Horizontal");

		public PluginSettingsView()
		{
			InitializeComponent();

			DataContextChanged += PluginSettingsView_DataContextChanged;

			_styles = new()
			{
				[typeof(TextBlock)]       = GetResource<Style>("Styles.TextBlock"),
				[typeof(AdvancedTextBox)] = GetResource<Style>("Styles.AdvancedTextBox"),
				[typeof(CheckBox)]        = GetResource<Style>("Styles.CheckBox"),
				[typeof(DatePicker)]      = GetResource<Style>("Styles.DatePicker"),
				[typeof(NormalButton)]    = GetResource<Style>("Styles.NormalButton"),
				[typeof(Rectangle)]       = GetResource<Style>("Styles.Delimeter.Horizontal"),
			};
		}

		private void PluginSettingsView_DataContextChanged(object sender, 
			DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is not PluginSettingsVM vm)
				return;

			DataContextChanged -= PluginSettingsView_DataContextChanged;

			Build(vm);
		}

		#region Building View

		private void Build(PluginSettingsVM vm)
		{
			var mainGrid = new Grid();
			mainGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
			mainGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
			mainGrid.RowDefinitions.Add(new() { Height = new GridLength(1, GridUnitType.Star) });
			mainGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
			mainGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });

			AddHeader(mainGrid, vm);
			AddBody(mainGrid, vm);
			AddFooter(mainGrid, vm);

			MainBorder.Child = mainGrid;

			ReturnButton.Style = GetResource<Style>("Styles.Button.Return");
			MainBorder.Style = GetResource<Style>("Styles.Border.Background");
		}

		private void AddHeader(Grid mainGrid, PluginSettingsVM vm)
		{
			var textBlock = new TextBlock
			{
				Style = _textBlockStyle,
				FontSize = 25,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Text = $"{vm.PluginInfo.Name} {GetResource<string>("Strings.Plugins.Settings")}"
			};

			textBlock.SetValue(Grid.RowProperty, 0);

			var delimeter = new Rectangle
			{
				Style = _delimeterStyle,
				Margin = new(0, 15, 0, 15)
			};

			delimeter.SetValue(Grid.RowProperty, 1);

			mainGrid.Children.Add(textBlock);
			mainGrid.Children.Add(delimeter);
		}

		private void AddBody(Grid mainGrid, PluginSettingsVM vm)
		{
			var scroll = new ScrollViewer()
			{
				Style = GetResource<Style>("Styles.ScrollViewer")
			};

			scroll.SetValue(Grid.RowProperty, 2);

			var grid = new Grid();
			for (var i = 0; i < vm.BindableProperties.Count; i++)
			{
				PutParameterToGrid(grid, vm.BindableProperties[i]);
			}

			scroll.Content = grid;

			var image = new Image()
			{
				Source = vm.PluginInfo.Image,
				Stretch = Stretch.Uniform,
				Opacity = 0.7,
				HorizontalAlignment = HorizontalAlignment.Right,
				VerticalAlignment = VerticalAlignment.Center,
				MinHeight = 200,
				Margin = new(0, 0, 25, 0)
			};

			image.SetValue(Grid.RowProperty, 2);
			mainGrid.Children.Add(image);

			mainGrid.Children.Add(scroll);
		}

		private void AddFooter(Grid mainGrid, PluginSettingsVM vm)
		{
			var delimeter = new Rectangle()
			{
				Style = _delimeterStyle,
				Margin = new(0, 15, 0, 15)
			};

			delimeter.SetValue(Grid.RowProperty, 3);

			var grid = new Grid()
			{
				Margin = new Thickness(20, 0, 20, 0)
			};

			grid.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });

			grid.SetValue(Grid.RowProperty, 4);

			var saveButton = new NormalButton()
			{
				Style = _normalButtonStyle,
				HorizontalAlignment = HorizontalAlignment.Right,
				Content = GetResource<string>("Strings.Buttons.Save")
			};
			saveButton.SetValue(Grid.ColumnProperty, 1);
			saveButton.SetBinding(
				Button.CommandProperty,
				vm,
				"SaveCommand",
				BindingMode.OneWay);

			var defaultButton = new NormalButton()
			{
				Style = _normalButtonStyle,
				HorizontalAlignment = HorizontalAlignment.Left,
				Content = GetResource<string>("Strings.Buttons.Default")
			};
			defaultButton.SetValue(Grid.ColumnProperty, 0);
			defaultButton.SetBinding(
				Button.CommandProperty,
				vm,
				"SetDefaultsCommand",
				BindingMode.OneWay);

			grid.Children.Add(saveButton);
			grid.Children.Add(defaultButton);

			mainGrid.Children.Add(delimeter);
			mainGrid.Children.Add(grid);
		}

		#endregion

		#region Settings table

		private void PutParameterToGrid(Grid grid, BindableProperty property)
		{
			var elements = property.CreateVisual(_styles);

			var nextRow = grid.RowDefinitions.Count;

			foreach(var element in elements)
			{
				element.SetValue(Grid.RowProperty, nextRow++);
				grid.RowDefinitions.Add(new() { Height = GridLength.Auto });

				grid.Children.Add(element);
			}

			elements[^1].SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 15));
		}

		#endregion

		#region Static

		private static T? GetResource<T>(string key) where T : class
		{
			return Application.Current.TryFindResource(key) as T;
		}

		#endregion
	}
}
