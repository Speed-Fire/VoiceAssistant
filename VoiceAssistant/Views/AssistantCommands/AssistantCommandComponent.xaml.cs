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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceAssistant.Entities;

namespace VoiceAssistant.Views.AssistantCommands
{
	/// <summary>
	/// Логика взаимодействия для AssistantActionComponent.xaml
	/// </summary>
	public partial class AssistantCommandComponent : UserControl
	{
		#region Dependency properties

		#region EnableCommand

		public static readonly DependencyProperty EnableCommandProperty=
			DependencyProperty.Register("EnableCommand", typeof(ICommand),
				typeof(AssistantCommandComponent), new PropertyMetadata(null, EnableCommandPropertyChanged));

		private static void EnableCommandPropertyChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var obj = d as AssistantCommandComponent;
			if (obj is null)
				return;

			obj.EnableCommandPropertyChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}

		private void EnableCommandPropertyChanged(ICommand oldValue, ICommand newValue)
		{
			EnableButton.Command = newValue;
		}

		public ICommand EnableCommand
		{
			get => (ICommand)GetValue(EnableCommandProperty);
			set => SetValue(EnableCommandProperty, value);
		}

		#endregion

		#region EditCommand

		public static readonly DependencyProperty EditCommandProperty =
			DependencyProperty.Register("EditCommand", typeof(ICommand),
				typeof(AssistantCommandComponent), new PropertyMetadata(null, EditCommandPropertyChanged));

		private static void EditCommandPropertyChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var obj = d as AssistantCommandComponent;
			if (obj is null)
				return;

			obj.EditCommandPropertyChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}

		private void EditCommandPropertyChanged(ICommand oldValue, ICommand newValue)
		{
			EditButton.Command = newValue;
		}

		public ICommand EditCommand
		{
			get => (ICommand)GetValue(EditCommandProperty);
			set => SetValue(EditCommandProperty, value);
		}

		#endregion

		#region DeleteCommand

		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(ICommand),
				typeof(AssistantCommandComponent), new PropertyMetadata(null, DeleteCommandPropertyChanged));

		private static void DeleteCommandPropertyChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var obj = d as AssistantCommandComponent;
			if (obj is null)
				return;

			obj.DeleteCommandPropertyChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}

		private void DeleteCommandPropertyChanged(ICommand oldValue, ICommand newValue)
		{
			DeleteButton.Command = newValue;
		}

		public ICommand DeleteCommand
		{
			get => (ICommand)GetValue(DeleteCommandProperty);
			set => SetValue(DeleteCommandProperty, value);
		}

		#endregion

		#endregion

		private readonly Storyboard _enableStoryboard;
		private readonly Storyboard _disableStoryboard;

		public AssistantCommandComponent()
		{
			InitializeComponent();

			_enableStoryboard = new Storyboard();
			_disableStoryboard = new Storyboard();

			InitAnimations();

			Loaded += AssistantCommandComponent_Loaded;
		}

		private void InitAnimations()
		{
			var enableAnimation = new DoubleAnimation(0.6, 0, new Duration(TimeSpan.FromSeconds(0.5)));
			var disableAnimation = new DoubleAnimation(0, 0.6, new Duration(TimeSpan.FromSeconds(0.5)));

			_enableStoryboard.Completed += Enabling_Completed;

			_enableStoryboard.Children.Add(enableAnimation);
			_disableStoryboard.Children.Add(disableAnimation);

			Storyboard.SetTarget(enableAnimation, EnabilityBorder);
			Storyboard.SetTarget(disableAnimation, EnabilityBorder);

			Storyboard.SetTargetProperty(enableAnimation, new(Border.OpacityProperty));
			Storyboard.SetTargetProperty(disableAnimation, new(Border.OpacityProperty));

			if (enableAnimation.CanFreeze)
				enableAnimation.Freeze();
			if (disableAnimation.CanFreeze)
				disableAnimation.Freeze();

			if (_enableStoryboard.CanFreeze)
				_enableStoryboard.Freeze();
			if(_disableStoryboard.CanFreeze)
				_disableStoryboard.Freeze();
		}

		private void AssistantCommandComponent_Loaded(object sender, RoutedEventArgs e)
		{
			var context = DataContext as INotifyPropertyChanged;
			if (context is null)
				return;

			context.PropertyChanged += Context_PropertyChanged;
			SynchronizeCommandEnability();
		}

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(AssistantCommandEntity.IsEnabled))
				return;

			SynchronizeCommandEnability();
		}

#nullable disable

		private void Enabling_Completed(object sender, EventArgs e)
		{
			var action = DataContext as AssistantCommandEntity;
			if (action is null)
				return;

			if (action.IsEnabled && EnabilityBorder.Visibility != Visibility.Collapsed)
				EnabilityBorder.Visibility = Visibility.Collapsed;
		}

		private void SynchronizeCommandEnability()
		{
			var action = DataContext as AssistantCommandEntity;
			if (action is null)
				return;

			EnabilityBorder.BeginAnimation(Border.OpacityProperty, null);

			var visibility = EnabilityBorder.Visibility;
			var opacity = EnabilityBorder.Opacity;
				
			if (action.IsEnabled)
			{
				_enableStoryboard.Begin(this);
			}
			else
			{
				EnabilityBorder.Visibility = Visibility.Visible;
				_disableStoryboard.Begin(this);
			}
		}
	}
}
