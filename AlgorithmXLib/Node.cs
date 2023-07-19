namespace PendleCodeMonkey.AlgorithmXLib
{
	public class Node
	{
		public Node Left { get; private set; }
		public Node Right { get; private set; }
		public Node Up { get; private set; }
		public Node Down { get; private set; }
		public ColumnNode? ColumnHeader { get; set; }

		public Node()
		{
			Left = Right = Up = Down = this;
			ColumnHeader = null;
		}

		public Node(ColumnNode col) : this() => ColumnHeader = col;

		// Attach the supplied node below this node
		internal Node AttachNodeDown(Node node)
		{
			node.Down = Down;
			node.Up = this;
			node.Down.Up = node;
			Down = node;
			return node;
		}

		// Attach the supplied node to the right of this node
		internal Node AttachNodeRight(Node node)
		{
			node.Right = Right;
			node.Left = this;
			node.Right.Left = node;
			Right = node;
			return node;
		}

		// Modify the Up and Down links so that this node is removed from the 'column' linked list.
		internal void UnlinkUpDown()
		{
			Up.Down = Down;
			Down.Up = Up;
		}

		// Modify the Up and Down links so that this node is restored into the 'column' linked list.
		internal void RelinkUpDown() => Up.Down = Down.Up = this;

		// Modify the Left and Right links so that this node is removed from the 'row' linked list.
		internal void UnlinkLeftRight()
		{
			Left.Right = Right;
			Right.Left = Left;
		}

		// Modify the Left and Right links so that this node is restored into the 'row' linked list.
		internal void RelinkLeftRight() => Left.Right = Right.Left = this;

	}
}
