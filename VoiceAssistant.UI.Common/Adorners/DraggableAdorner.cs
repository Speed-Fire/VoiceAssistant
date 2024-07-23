using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using VoiceAssistant.UI.Common.Helpers;

namespace VoiceAssistant.UI.Common.Adorners
{
	public class DraggableAdorner : Adorner
	{
		private readonly FrameworkElement _element;
		private readonly Brush _elementBrush;

		private Point _mousePosition;

		private Rect _elementRect;
		private Point _location;
		private Point _offset;

		public DraggableAdorner(FrameworkElement adornedElement, Point offset) 
			: base(adornedElement)
		{
			IsHitTestVisible = false;

			_element = adornedElement;
			_offset = offset;

			_elementRect = new(adornedElement.RenderSize);
			_elementBrush = new VisualBrush(adornedElement) { Opacity = 0.8 };
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var p = _location;
			p.Offset(-_offset.X, -_offset.Y);

			_elementRect.Location = p;

			drawingContext.DrawRectangle(_elementBrush, null, _elementRect);	
		}

		public void UpdatePosition(Point location)
		{
			this._location = location;
			this.InvalidateVisual();
		}

		public void MoveToMousePosition()
		{
			MouseHelper.GetMousePos(ref _mousePosition);

			var npos = _element.PointFromScreen(_mousePosition);

			UpdatePosition(npos);
		}
	}
}
