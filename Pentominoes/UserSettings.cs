using System.Windows;

namespace PendleCodeMonkey.Pentominoes
{
	public class UserSettings
	{
		public class WindowStateSettings
		{
			public WindowState State { get; set; }
			public Visibility Visibility { get; set; }
			public double Top { get; set; }
			public double Left { get; set; }
			public double Height { get; set; }
			public double Width { get; set; }
		}

		public UserSettings()
		{
			AlphaLabelEnabled = false;
			MaxSolutionsEnabled = false;
			MaxSolutions = 100;
		}

		public bool AlphaLabelEnabled { get; set; }
		public bool MaxSolutionsEnabled { get; set; }
		public int MaxSolutions { get; set; }
		public WindowStateSettings? MainWindowStateSettings { get; set; }

		public static WindowStateSettings GetWindowStateSettings(Window wnd)
		{
			WindowStateSettings windowStateSettings = new ();
			if (wnd != null)
			{
				windowStateSettings.State = wnd.WindowState;
				windowStateSettings.Visibility = wnd.Visibility;
				if (wnd.WindowState == WindowState.Maximized)
				{
					windowStateSettings.Top = wnd.RestoreBounds.Top;
					windowStateSettings.Left = wnd.RestoreBounds.Left;
					windowStateSettings.Height = wnd.RestoreBounds.Height;
					windowStateSettings.Width = wnd.RestoreBounds.Width;
				}
				else
				{
					windowStateSettings.Top = wnd.Top;
					windowStateSettings.Left = wnd.Left;
					windowStateSettings.Height = wnd.Height;
					windowStateSettings.Width = wnd.Width;
				}
			}

			return windowStateSettings;
		}

		public static void LoadWindowStateSettings(WindowStateSettings? windowStateSettings, Window wnd)
		{
			if (wnd != null && windowStateSettings != null)
			{
				wnd.WindowState = windowStateSettings.State;
				wnd.Visibility = windowStateSettings.Visibility;
				wnd.Top = windowStateSettings.Top;
				wnd.Left = windowStateSettings.Left;
				wnd.Height = windowStateSettings.Height;
				wnd.Width = windowStateSettings.Width;
			}
		}
	}
}
