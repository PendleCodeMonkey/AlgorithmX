namespace PendleCodeMonkey.AlgorithmXLib
{
	public class ColumnNode : Node
	{
		public string Name { get; private set; }
		public int Size { get; private set; }

		public ColumnNode(string colName) : base()
		{
			Size = 0;
			Name = colName;
			ColumnHeader = this;
		}

		internal void IncrementSize() => Size++;
		internal void DecrementSize() => Size--;
		internal void SetSize(int size) => Size = size;

		internal void Cover()
		{
			UnlinkLeftRight();
			for (Node i = Down; i != this; i = i.Down)
			{
				for (Node j = i.Right; j != i; j = j.Right)
				{
					j.UnlinkUpDown();
					j.ColumnHeader!.DecrementSize();
				}
			}
		}

		internal void Uncover()
		{
			for (Node i = Up; i != this; i = i.Up)
			{
				for (Node j = i.Left; j != i; j = j.Left)
				{
					j.ColumnHeader!.IncrementSize();
					j.RelinkUpDown();
				}
			}
			RelinkLeftRight();
		}
	}
}
