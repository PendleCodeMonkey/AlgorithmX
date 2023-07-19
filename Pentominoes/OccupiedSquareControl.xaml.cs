using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PendleCodeMonkey.Pentominoes
{
	/// <summary>
	/// Interaction logic for OccupiedSquareControl.xaml
	/// </summary>
	public partial class OccupiedSquareControl : UserControl
	{
		public int PentominoID { get; set; }
		public (int north, int east, int south, int west) NeighbouringIDs { get; set; }
		public Color PentominoColour { get; set; }
		public bool ShowAlphaLabel { get; set; }

		public OccupiedSquareControl()
		{
			InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			SolidColorBrush penBrush = new(Color.FromArgb(32, 0, 0, 0));
			drawingContext.DrawRectangle(new SolidColorBrush(PentominoColour), new Pen(penBrush, 0.25), new Rect(0, 0, ActualWidth, ActualHeight));
			Pen pen = new(Brushes.Black, 0.5);
			double top = 0.0;
			double left = 0.0;
			double bottom = ActualHeight;
			double right = ActualWidth;
			if (PentominoID != NeighbouringIDs.north)
			{
				drawingContext.DrawLine(pen, new Point(left, top), new Point(right, top));
			}
			if (PentominoID != NeighbouringIDs.west)
			{
				drawingContext.DrawLine(pen, new Point(left, top), new Point(left, bottom));
			}
			if (PentominoID != NeighbouringIDs.south)
			{
				drawingContext.DrawLine(pen, new Point(left, bottom), new Point(right, bottom));
			}
			if (PentominoID != NeighbouringIDs.east)
			{
				drawingContext.DrawLine(pen, new Point(right, top), new Point(right, bottom));
			}

			if (ShowAlphaLabel)
			{
				FontFamily courier = new("Courier New");
				Typeface courierTypeface = new(courier, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
				string alphaID = Pentominoes.MapNumericIDToAlpha[PentominoID];
				SolidColorBrush textBrush = new(Color.FromArgb(128, 0, 0, 0));
				FormattedText ft2 = new(alphaID,
										System.Globalization.CultureInfo.CurrentCulture,
										FlowDirection.LeftToRight,
										courierTypeface,
										12.0,
										textBrush,
										VisualTreeHelper.GetDpi(this).PixelsPerDip);


				double x = (ActualWidth - ft2.Width) / 2;
				double y = (ActualHeight - ft2.Height) / 2;
				drawingContext.DrawText(ft2, new Point(x, y));
			}
		}
	}
}
