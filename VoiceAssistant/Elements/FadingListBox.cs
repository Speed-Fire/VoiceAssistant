using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace VoiceAssistant.Elements
{
	internal class FadingListBox : ListBox
	{
        private readonly Color COLOR_BLACK = new() { R = 0, G = 0, B = 0, A = 255 };
        private readonly Color COLOR_TRANSPARENT = new() { A = 0 };

        #region Dependency properties

        #region Threshold

        public readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register("Threshold", typeof(double), typeof(FadingListBox),
                new UIPropertyMetadata(0.2));

        public double Threshold
        {
            get => (double)GetValue(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }

		#endregion

		#region EdgeGap

		public readonly DependencyProperty EdgeGapProperty =
			DependencyProperty.Register("EdgeGap", typeof(double), typeof(FadingListBox),
				new UIPropertyMetadata(0.5));

		public double EdgeGap
		{
			get => (double)GetValue(EdgeGapProperty);
			set => SetValue(EdgeGapProperty, value);
		}

		#endregion

		#endregion

		public FadingListBox()
        {
			Loaded += FadingListBox_Loaded; 
        }

		private void FadingListBox_Loaded(object sender, RoutedEventArgs e)
		{
			this.AddHandler(ScrollViewer.ScrollChangedEvent,
				new ScrollChangedEventHandler(OnScroll));

			UpdateElementsFading();
		}

		private void OnScroll(object sender, ScrollChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

			UpdateElementsFading();
		}

        private void UpdateElementsFading()
        {
			var count = this.ItemContainerGenerator.Items.Count;
			var listHeight = this.ActualHeight;
			var gap = listHeight * EdgeGap;
			for (int i = 0; i < count; i++)
			{
				var obj = this.ItemContainerGenerator.ContainerFromIndex(i);

				if (obj is not Control control)
					continue;

                if(i == 0)
                {
					control.Margin = new Thickness(0, gap, 0, 0);
				}
                else if(i == count - 1)
                {
					control.Margin = new Thickness(0, 0, 0, gap);
				}

				UpdateElementFading(control);
			}
		}

        private void UpdateElementFading(Control control)
        {
            if (!this.IsAncestorOf(control))
                return;

            var listHeight = this.ActualHeight;
            var elementHeight = control.ActualHeight;

            var pos = control.TransformToAncestor(this)
                .Transform(new Point(0, 0));

            if (pos.Y > listHeight || pos.Y + elementHeight < 0)
                return;

            SetDefaultOpacityMask(control);

            var topPoint = ((LinearGradientBrush)control.OpacityMask).GradientStops[0];
            var middlePoint = ((LinearGradientBrush)control.OpacityMask).GradientStops[1];
            var bottomPoint = ((LinearGradientBrush)control.OpacityMask).GradientStops[2];

            var threshold = Threshold;
            var topThreshold = listHeight * threshold;
            var bottomThreshold = listHeight - topThreshold;
            var topDisapperThreshold = topThreshold - elementHeight;
            var bottomDisapperThreshold = bottomThreshold + elementHeight;

            if(pos.Y >= bottomDisapperThreshold ||
				pos.Y + elementHeight <= topDisapperThreshold)
            {
                control.Opacity = 0;
                return;
			}

			control.Opacity = 1;

            double referenceThreshold;

			if (pos.Y + elementHeight >= bottomDisapperThreshold) // bottom disappear area
			{
				topPoint.Color = COLOR_BLACK;
				middlePoint.Color = COLOR_TRANSPARENT;
				bottomPoint.Color = COLOR_TRANSPARENT;

                var betweenThresholdPart = bottomDisapperThreshold - pos.Y;
                control.Opacity = betweenThresholdPart / elementHeight;

                referenceThreshold = bottomDisapperThreshold;
			}
			else if (pos.Y + elementHeight >= bottomThreshold)        // bottom area
			{
				topPoint.Color = COLOR_BLACK;
				middlePoint.Color = COLOR_BLACK;
				bottomPoint.Color = COLOR_BLACK;

				var betweenThresholdPart = bottomDisapperThreshold - (pos.Y + elementHeight);
				var opacity = (byte)(betweenThresholdPart / elementHeight * 255);

				var color = COLOR_BLACK;
				color.A = opacity;

				bottomPoint.Color = color;

				referenceThreshold = bottomThreshold;
			}
			else if (pos.Y <= topDisapperThreshold)              // top disappear area
			{
				topPoint.Color = COLOR_TRANSPARENT;
				middlePoint.Color = COLOR_TRANSPARENT;
				bottomPoint.Color = COLOR_BLACK;

				var betweenThresholdPart = (pos.Y + elementHeight) - topDisapperThreshold;
				control.Opacity = betweenThresholdPart / elementHeight;

				referenceThreshold = topDisapperThreshold;
			}
			else if(pos.Y <= topThreshold)                       // top area              
            {
				//topPoint.Color = COLOR_TRANSPARENT;
				middlePoint.Color = COLOR_BLACK;
				bottomPoint.Color = COLOR_BLACK;

				var betweenThresholdPart = pos.Y - topDisapperThreshold;
                var opacity = (byte)(betweenThresholdPart / elementHeight * 255);

                var color = COLOR_BLACK;
                color.A = opacity;

                topPoint.Color = color;

				referenceThreshold = topThreshold;
			}
            else                                                 // middle area
            {
				topPoint.Color = COLOR_BLACK;
				middlePoint.Color = COLOR_BLACK;
				middlePoint.Offset = 1;

                return;
            }

			var beforeTreeshold = referenceThreshold - pos.Y;
			var koeficient = beforeTreeshold / elementHeight;

			middlePoint.Offset = koeficient;
		}

        private void SetDefaultOpacityMask(Control control)
        {
            if (control.OpacityMask is not null)
                return;

            var brush = new LinearGradientBrush()
            {
                StartPoint = new(0.5, 0),
                EndPoint = new(0.5, 1),
                GradientStops =
                    [
                        new(COLOR_BLACK, 0),
                        new(COLOR_BLACK, 0.5),
                        new(COLOR_TRANSPARENT, 1),
                    ]
            };

            control.OpacityMask = brush;  
        }
    }
}
