using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PendleCodeMonkey.Pentominoes
{
	internal class DataModel : INotifyPropertyChanged
	{
		private int _maxSolutions;

		public int MaxSolutions
		{
			get
			{
				return _maxSolutions;
			}

			set
			{
				if (value != _maxSolutions)
				{
					_maxSolutions = value;
					NotifyPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		// This method is called by the Set accessor of each property.  
		// The CallerMemberName attribute that is applied to the optional propertyName  
		// parameter causes the property name of the caller to be substituted as an argument.  
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
