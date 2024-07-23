using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoiceAssistant.UI.Common.Helpers
{
	public static partial class MouseHelper
	{
		[LibraryImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool GetCursorPos(ref Win32Point p);

		[StructLayout(LayoutKind.Sequential)]
		internal struct Win32Point
		{
			public Int32 X;
			public Int32 Y;
		}

		public static Point GetMousePos()
		{
			Win32Point w32Mouse = new();
			GetCursorPos(ref w32Mouse);

			return new Point(w32Mouse.X, w32Mouse.Y);
		}

		public static void GetMousePos(ref Point point)
		{
			Win32Point w32Mouse = new();
			GetCursorPos(ref w32Mouse);

			point.X = w32Mouse.X;
			point.Y = w32Mouse.Y;
		}
	}
}
