using System.Collections.Generic;

namespace PendleCodeMonkey.Pentominoes
{
	internal class PentominoConfiguration
	{
		public int ID { get; set; }
		public char AlphaID { get; set; }
		public List<(int x, int y)> Coords { get; set; }

		public PentominoConfiguration()
		{
			Coords = new List<(int x, int y)>();
		}
	}
}
