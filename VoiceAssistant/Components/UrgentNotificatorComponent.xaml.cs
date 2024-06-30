using CommunityToolkit.Mvvm.Messaging;
using Synergy.WPF.Common.AttachedProperties;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceAssistant.Notifications;

namespace VoiceAssistant.Components
{
	/// <summary>
	/// Логика взаимодействия для UrgentNotificatorComponent.xaml
	/// </summary>
	public partial class UrgentNotificatorComponent : UserControl
	{
		private readonly List<DependencyObject> _hitResultList = [];

		public UrgentNotificatorComponent(UrgentNotificationHandler notificationHandler)
		{
			InitializeComponent();

			DataContext = notificationHandler;

			Loaded += UrgentNotificatorComponent_Loaded;
			Unloaded += UrgentNotificatorComponent_Unloaded;
		}

		private void UrgentNotificatorComponent_Loaded(object sender, RoutedEventArgs e)
		{
			AddMouseHandler();
		}

		private void UrgentNotificatorComponent_Unloaded(object sender, RoutedEventArgs e)
		{
			RemoveMouseHandler();
		}

		#region Handler registrations

		private void AddMouseHandler()
		{
			Application.Current.MainWindow
				.AddHandler(Mouse.PreviewMouseDownEvent,
					new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
		}

		private void RemoveMouseHandler()
		{
			Application.Current.MainWindow
				.RemoveHandler(Mouse.PreviewMouseDownEvent,
					new MouseButtonEventHandler(HandleClickOutsideOfControl));
		}

		#endregion

		#region Click outside of control handler

		private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
		{
			if (!GetShownStatus()) return;

			var pt = e.GetPosition((UIElement)sender);
			_hitResultList.Clear();

			//Retrieving all the elements under the cursor
			VisualTreeHelper.HitTest(Application.Current.MainWindow, null,
				new HitTestResultCallback(MyHitTestResultCallback),
				new PointHitTestParameters(pt));

			//Testing if the page is under the cursor
			if (!_hitResultList.Contains(UrgentBorder))
			{
				SetShownStatus(false);
			}
		}

		#region Callbacks

		private HitTestResultBehavior MyHitTestResultCallback(HitTestResult result)
		{
			_hitResultList.Add(result.VisualHit);
			return HitTestResultBehavior.Continue;
		}

		#endregion

		#endregion

		#region Internal

		private bool GetShownStatus()
		{
			return (bool)this.GetValue(AnimateSlideInFromRightProperty.ValueProperty);
		}

		private void SetShownStatus(bool value)
		{
			this.SetValue(AnimateSlideInFromRightProperty.ValueProperty, value);
		}

		#endregion
	}
}
