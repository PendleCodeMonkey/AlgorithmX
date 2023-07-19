using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PendleCodeMonkey.Pentominoes
{
	/// <summary>
	/// Interaction logic for ExcludedSquareControl.xaml
	/// </summary>
	public partial class ExcludedSquareControl : UserControl
	{
		public (int north, int east, int south, int west) NeighbouringIDs { get; set; }

		public ExcludedSquareControl()
		{
			InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			drawingContext.DrawRectangle(new SolidColorBrush(Colors.SlateBlue), null, new Rect(0, 0, ActualWidth, ActualHeight));
			Pen pen = new(Brushes.Black, 0.5);
			double top = 0.0;
			double left = 0.0;
			double bottom = ActualHeight;
			double right = ActualWidth;
			if (0 != NeighbouringIDs.north)
			{
				drawingContext.DrawLine(pen, new Point(left, top), new Point(right, top));
			}
			if (0 != NeighbouringIDs.west)
			{
				drawingContext.DrawLine(pen, new Point(left, top), new Point(left, bottom));
			}
			if (0 != NeighbouringIDs.south)
			{
				drawingContext.DrawLine(pen, new Point(left, bottom), new Point(right, bottom));
			}
			if (0 != NeighbouringIDs.east)
			{
				drawingContext.DrawLine(pen, new Point(right, top), new Point(right, bottom));
			}
		}
	}
}
