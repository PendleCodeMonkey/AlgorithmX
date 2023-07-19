namespace PendleCodeMonkey.AlgorithmXLib
{
	// Class that implements Donald Knuth's Algorithm X for solving exact cover problems using Dancing Links (DLX)
	// See https://arxiv.org/pdf/cs/0011047.pdf and https://en.wikipedia.org/wiki/Dancing_Links
	public class AlgoX
	{
		// Enumeration of the ways in which columns can be labelled/named (numerically or alphabetically)
		public enum ColumnNaming
		{
			Numeric = 0,
			Alphabetic = 1
		}

		private ColumnNode? Header { get; set; }
		private int NumSolutions { get; set; }
		private int MaxSolutions { get; set; }
		private ColumnNaming ColNaming { get; set; }
		private Stack<Node> Solution { get; set; }
		private Action<List<Node>>? SolutionHandler { get; set; }

		public AlgoX(bool[,] exactCover, Action<List<Node>>? handler, ColumnNaming colNaming = ColumnNaming.Numeric)
		{
			NumSolutions = 0;
			ColNaming = colNaming;
			Header = GenerateDLXNodes(exactCover, ColNaming);
			Solution = new Stack<Node>();
			SolutionHandler = handler;
		}

		public void Solve(int maxSolutions, CancellationToken cancelToken)
		{
			MaxSolutions = maxSolutions;
			Search(0, cancelToken);
		}


		// Recursive method that searches for solutions.
		private void Search(int level, CancellationToken cancelToken)
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			if (Header!.Right == Header)
			{
				// We have a solution.
				NumSolutions++;
				// Invoke the handler method (if any) so the client can handle the solution we have found.
				SolutionHandler?.Invoke(Solution.Reverse().ToList());
			}
			else if (NumSolutions < MaxSolutions)
			{
				ColumnNode? c = SelectColumnNode(Header);
				if (c is not null)
				{
					c.Cover();

					for (Node r = c.Down; r != c; r = r.Down)
					{
						Solution.Push(r);

						for (Node j = r.Right; j != r; j = j.Right)
						{
							j.ColumnHeader?.Cover();
						}

						Search(level + 1, cancelToken);

						r = Solution.Pop();
						c = r.ColumnHeader;

						for (Node j = r.Left; j != r; j = j.Left)
						{
							j.ColumnHeader?.Uncover();
						}
					}
					c.Uncover();
				}
			}
		}

		// Select the column node with the smallest size.
		internal static ColumnNode? SelectColumnNode(ColumnNode header)
		{
			int min = int.MaxValue;
			ColumnNode? colNode = null;
			for (ColumnNode col = (ColumnNode)header!.Right; col != header; col = (ColumnNode)col.Right)
			{
				if (col.Size < min)
				{
					min = col.Size;
					colNode = col;
				}
			}
			return colNode;
		}

		// Generate the "Dancing Links" nodes from the supplied exact cover matrix.
		// Returns the root column header node.
		internal static ColumnNode? GenerateDLXNodes(bool[,] exactCover, ColumnNaming colNaming)
		{
			int numRows = exactCover.GetLength(0);
			int numColumns = exactCover.GetLength(1);

			ColumnNode? headerNode = new("header");
			List<ColumnNode> columnNodes = new();

			for (int col = 0; col < numColumns; col++)
			{
				string colName = colNaming == ColumnNaming.Numeric ? col.ToString() : $"{(char)('A' + col)}";
				ColumnNode node = new(colName);
				columnNodes.Add(node);
				headerNode = (ColumnNode)headerNode.AttachNodeRight(node);
			}
			headerNode = headerNode.Right.ColumnHeader;

			for (int row = 0; row < numRows; row++)
			{
				Node? prevNode = null;
				for (int col = 0; col < numColumns; col++)
				{
					if (exactCover[row, col])
					{
						ColumnNode column = columnNodes[col];
						Node newNode = new(column);
						prevNode ??= newNode;
						column.Up.AttachNodeDown(newNode);
						prevNode = prevNode.AttachNodeRight(newNode);
						column.IncrementSize();
					}
				}
			}

			headerNode?.SetSize(numColumns);

			return headerNode;
		}
	}
}