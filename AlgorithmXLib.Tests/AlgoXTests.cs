namespace PendleCodeMonkey.AlgorithmXLib.Tests
{
	public class AlgoXTests
	{
		private readonly bool[,] exampleExactCover = {
													{ false, false, true, false, true, true, false },
													{ true, false, false, true, false, false, true },
													{ false, true, true, false, false, true, false },
													{ true, false, false, true, false, false, false },
													{ false, true, false, false, false, false, true },
													{ false, false, false, true, true, false, true }
													};



		[Fact]
		public void SuccessfullyCreatesNewNode()
		{
			Node node = new();

			Assert.NotNull(node);
			Assert.Equal(node.Left, node);
			Assert.Equal(node.Right, node);
			Assert.Equal(node.Up, node);
			Assert.Equal(node.Down, node);
			Assert.Null(node.ColumnHeader);
		}

		[Fact]
		public void AttachNodeRight_YieldsCorrectLinks()
		{
			Node node = new();
			Node node2 = new();

			var temp = node.AttachNodeRight(node2);

			Assert.Same(temp, node2);
			Assert.Same(node2.Right, node);
			Assert.Same(node.Right, node2);
			Assert.Same(node2.Left, node);
			Assert.Same(node.Left, node2);
		}

		[Fact]
		public void AttachNodeDown_YieldsCorrectLinks()
		{
			Node node = new();
			Node node2 = new();

			var temp = node.AttachNodeDown(node2);

			Assert.Same(temp, node2);
			Assert.Same(node2.Down, node);
			Assert.Same(node.Down, node2);
			Assert.Same(node2.Up, node);
			Assert.Same(node.Up, node2);
		}

		[Fact]
		public void UnlinkUpDown_CorrectlyUnlinksNode()
		{
			Node node = new();
			Node node2 = new();
			Node node3 = new();
			_ = node.AttachNodeDown(node2);
			_ = node2.AttachNodeDown(node3);

			// Unlink node2
			node2.UnlinkUpDown();

			// Assert that node and node3 link to each other (bypassing the unlinked node2)
			Assert.Same(node.Down, node3);
			Assert.Same(node3.Up, node);
		}

		[Fact]
		public void RelinkUpDown_CorrectlyRelinksNode()
		{
			Node node = new();
			Node node2 = new();
			Node node3 = new();
			_ = node.AttachNodeDown(node2);
			_ = node2.AttachNodeDown(node3);

			// Unlink node2 then re-link it.
			node2.UnlinkUpDown();
			node2.RelinkUpDown();

			// Assert that node and node3 correctly link to the re-linked node2
			Assert.Same(node.Down, node2);
			Assert.Same(node3.Up, node2);
		}

		[Fact]
		public void UnlinkLeftRight_CorrectlyUnlinksNode()
		{
			Node node = new();
			Node node2 = new();
			Node node3 = new();
			_ = node.AttachNodeRight(node2);
			_ = node2.AttachNodeRight(node3);

			// Unlink node2
			node2.UnlinkLeftRight();

			// Assert that node and node3 link to each other (bypassing the unlinked node2)
			Assert.Same(node.Right, node3);
			Assert.Same(node3.Left, node);
		}

		[Fact]
		public void RelinkLeftRight_CorrectlyRelinksNode()
		{
			Node node = new();
			Node node2 = new();
			Node node3 = new();
			_ = node.AttachNodeRight(node2);
			_ = node2.AttachNodeRight(node3);

			// Unlink node2 then re-link it.
			node2.UnlinkLeftRight();
			node2.RelinkLeftRight();

			// Assert that node and node3 correctly link to the re-linked node2
			Assert.Same(node.Right, node2);
			Assert.Same(node3.Left, node2);
		}

		[Fact]
		public void SuccessfullyCreatesNewColumnNode()
		{
			ColumnNode colNode = new("ColName");

			Assert.NotNull(colNode);
			Assert.Equal(colNode.Left, colNode);
			Assert.Equal(colNode.Right, colNode);
			Assert.Equal(colNode.Up, colNode);
			Assert.Equal(colNode.Down, colNode);
			Assert.Equal(colNode.ColumnHeader, colNode);
			Assert.Equal("ColName", colNode.Name);
			Assert.Equal(0, colNode.Size);
		}

		[Fact]
		public void SetSize_CorrectlySetsSize()
		{
			ColumnNode colNode = new("ColName");

			colNode.SetSize(5);

			Assert.Equal(5, colNode.Size);
		}

		[Fact]
		public void IncrementSize_AddsOneToSize()
		{
			ColumnNode colNode = new("ColName");

			colNode.IncrementSize();

			Assert.Equal(1, colNode.Size);
		}

		[Fact]
		public void DecrementSize_SubtractsOneFromSize()
		{
			ColumnNode colNode = new("ColName");

			colNode.SetSize(5);
			colNode.DecrementSize();

			Assert.Equal(4, colNode.Size);
		}

		[Fact]
		public void GenerateDLXNodes_GeneratesNodes()
		{
			var header = AlgoX.GenerateDLXNodes(exampleExactCover, AlgoX.ColumnNaming.Alphabetic);
			Assert.NotNull(header);
			Assert.Equal("header", header.Name);
			Assert.Equal(7, header.Size);
		}

		[Fact]
		public void DLXNodes_ColumnsHaveCorrectNameAndSize()
		{
			ColumnNode? header = AlgoX.GenerateDLXNodes(exampleExactCover, AlgoX.ColumnNaming.Alphabetic);
			var node1 = header!.Right;
			var node2 = node1.Right;
			var node3 = node2.Right;
			var node4 = node3.Right;
			var node5 = node4.Right;
			var node6 = node5.Right;
			var node7 = node6.Right;

			Assert.Equal("A", node1.ColumnHeader!.Name);
			Assert.Equal(2, node1.ColumnHeader.Size);
			Assert.Equal("B", node2.ColumnHeader!.Name);
			Assert.Equal(2, node2.ColumnHeader.Size);
			Assert.Equal("C", node3.ColumnHeader!.Name);
			Assert.Equal(2, node3.ColumnHeader.Size);
			Assert.Equal("D", node4.ColumnHeader!.Name);
			Assert.Equal(3, node4.ColumnHeader.Size);
			Assert.Equal("E", node5.ColumnHeader!.Name);
			Assert.Equal(2, node5.ColumnHeader.Size);
			Assert.Equal("F", node6.ColumnHeader!.Name);
			Assert.Equal(2, node6.ColumnHeader.Size);
			Assert.Equal("G", node7.ColumnHeader!.Name);
			Assert.Equal(3, node7.ColumnHeader.Size);
		}

		[Fact]
		public void SelectColumnNode_SelectsFirstColumnWithSmallestSize()
		{
			ColumnNode? header = AlgoX.GenerateDLXNodes(exampleExactCover, AlgoX.ColumnNaming.Alphabetic);
			var colNode = AlgoX.SelectColumnNode(header!);

			Assert.Equal("A", colNode!.ColumnHeader!.Name);
			Assert.Equal(2, colNode.ColumnHeader.Size);
		}

		[Fact]
		public void ColumnNodeCover_UnlinksCorrectNodes()
		{
			ColumnNode? header = AlgoX.GenerateDLXNodes(exampleExactCover, AlgoX.ColumnNaming.Alphabetic);
			var colNode = (ColumnNode)header!.Right;

			colNode.Cover();
			var node = header!.Right.Right.Right;

			Assert.Equal("B", ((ColumnNode)header!.Right).Name);
			Assert.Equal("D", node.ColumnHeader!.Name);
			Assert.Equal(1, node.ColumnHeader!.Size);
		}

		[Fact]
		public void ColumnNodeUncover_ReversesCover()
		{
			ColumnNode? header = AlgoX.GenerateDLXNodes(exampleExactCover, AlgoX.ColumnNaming.Alphabetic);
			var colNode = (ColumnNode)header!.Right;

			colNode.Cover();
			colNode.Uncover();

			var node = header!.Right.Right.Right.Right;

			Assert.Equal("A", ((ColumnNode)header!.Right).Name);
			Assert.Equal("D", node.ColumnHeader!.Name);
			Assert.Equal(3, node.ColumnHeader!.Size);
		}

		[Fact]
		public void Solve_YieldsCorrectSolution()
		{
			AlgoX algoX = new(exampleExactCover, HandleSolution, AlgoX.ColumnNaming.Alphabetic);
			var cancelSource = new CancellationTokenSource();
			algoX.Solve(1, cancelSource.Token);

			static void HandleSolution(List<Node> solution)
			{
				List<string> results = new();
				foreach (Node n in solution)
				{
					string ret = "";
					ret += n.ColumnHeader!.Name;
					Node tmp = n.Right;
					while (tmp != n)
					{
						ret += tmp.ColumnHeader!.Name;
						tmp = tmp.Right;
					}
					results.Add(ret);
				}
				Assert.Equal(3, solution.Count);
				Assert.Equal("AD", results[0]);
				Assert.Equal("EFC", results[1]);
				Assert.Equal("BG", results[2]);
			}
		}
	}
}