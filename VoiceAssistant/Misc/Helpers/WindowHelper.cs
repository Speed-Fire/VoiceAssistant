using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;



#nullable disable

namespace VoiceAssistant.Misc.Helpers
{
	internal static class WindowHelper
	{
		private static Window _wnd;
		private static Point _oldPosition;
		private static Size _oldSize;

		private static bool _maximized = false;
		public static bool IsMaximized => _maximized;

		public static void Init(Window wnd)
		{
			_wnd = wnd;
		}

		public static void DoMaximize()
		{
			if (_maximized)
			{
				Normalize();
			}
			else
			{
				Maximize();
			}

			_maximized = !_maximized;
		}

		private static void Maximize()
		{
			_oldPosition = new(_wnd.Left, _wnd.Top);
			_oldSize = new(_wnd.Width, _wnd.Height);

			_wnd.Width = SystemParameters.WorkArea.Width;
			_wnd.Height = SystemParameters.WorkArea.Height;

			_wnd.Top = 0;
			_wnd.Left = 0;
		}

		private static void Normalize()
		{
			_wnd.Width = _oldSize.Width;
			_wnd.Height = _oldSize.Height;

			_wnd.Left = _oldPosition.X;
			_wnd.Top = _oldPosition.Y;
		}

		public static void NormilizeAndAdjustToMouse()
		{
			if (!_maximized) return;

			var locpos = Mouse.GetPosition(_wnd);
			var abspos = _wnd.PointToScreen(locpos);

			var deltaX = abspos.X / _wnd.ActualWidth;
			var deltaY = abspos.Y / _wnd.ActualHeight;

			Normalize();

			_wnd.Left = abspos.X - _wnd.ActualWidth * deltaX;
			_wnd.Top = abspos.Y - _wnd.ActualHeight * deltaY;

			_maximized = false;
		}
	}
}
