using PendleCodeMonkey.AlgorithmXLib;
using static PendleCodeMonkey.AlgorithmXLib.AlgoX;

namespace PendleCodeMonkey.AlgorithmXConsoleApp
{
	internal class Sudoku
	{
		protected int BoardSize = 9; // Size of the board (i.e. 9 x 9)
		protected int BoxSize = 3; // Size of each box (i.e. 3 x 3)

		private int[,]? _sudokuPuzzle;

		public Sudoku()
		{
		}

		public void Solve(int[,] sudoku, int maxSol, ColumnNaming colNaming)
		{
			_sudokuPuzzle = sudoku;
			bool[,] exactCover = MakeSudokuExactCover(sudoku);
			AlgoX dlx = new(exactCover, HandleSolution, colNaming);
			var cancelSource = new CancellationTokenSource();
			dlx.Solve(maxSol, cancelSource.Token);
		}

		public void HandleSolution(List<Node> solution)
		{
			int[,] result = ParseSolution(solution);
			OutputSolution(result);
		}

		// Parse the solution (a list of Node objects) to get the solution of the sudoku in the required format.
		private int[,] ParseSolution(List<Node> solution)
		{
			int[,] result = new int[BoardSize, BoardSize];
			foreach (Node n in solution)
			{
				Node rcNode = n;
				int min = int.Parse(rcNode.ColumnHeader!.Name);
				for (Node tmp = n.Right; tmp != n; tmp = tmp.Right)
				{
					int val = int.Parse(tmp.ColumnHeader!.Name);
					if (val < min)
					{
						min = val;
						rcNode = tmp;
					}
				}
				int ans1 = int.Parse(rcNode.ColumnHeader.Name);
				int ans2 = int.Parse(rcNode.Right.ColumnHeader!.Name);
				int row = ans1 / BoardSize;
				int column = ans1 % BoardSize;
				int number = (ans2 % BoardSize) + 1;
				result[row, column] = number;
			}
			return result;
		}

		private void OutputSolution(int[,] solution)
		{
			if (_sudokuPuzzle == null)
			{
				return;
			}
			int numRows = solution.GetLength(0);
			int numColumns = solution.GetLength(1);
			Console.WriteLine("┌─┬─┬─┬─┬─┬─┬─┬─┬─┐\t\t┌─┬─┬─┬─┬─┬─┬─┬─┬─┐");
			for (int row = 0; row < numRows; row++)
			{
				string line = "│";
				for (int col = 0; col < numColumns; col++)
				{

					line += (_sudokuPuzzle[row, col] == 0 ? " " : _sudokuPuzzle[row, col]) + "│";
				}
				if (row == 4)
				{
					line += "\t--->\t│";
				}
				else
				{
					line += "\t\t│";
				}
				for (int col = 0; col < numColumns; col++)
				{
					line += solution[row, col] + "│";
				}
				Console.WriteLine(line);
				if (row < numRows - 1)
				{
					Console.WriteLine("├─┼─┼─┼─┼─┼─┼─┼─┼─┤\t\t├─┼─┼─┼─┼─┼─┼─┼─┼─┤");
				}
			}

			Console.WriteLine("└─┴─┴─┴─┴─┴─┴─┴─┴─┘\t\t└─┴─┴─┴─┴─┴─┴─┴─┴─┘");
			Console.WriteLine();
		}

		// Generate the exact cover matrix for the given Sudoku puzzle (for solving using the AlgorithmX functionality).
		private bool[,] MakeSudokuExactCover(int[,] sudoku)
		{
			// Local method that calculates an index value based on the row, column, and number combination.
			int Idx(int row, int col, int num) => ((row - 1) * BoardSize * BoardSize) + ((col - 1) * BoardSize) + (num - 1);

			int ecRows = BoardSize * BoardSize * BoardSize;
			int ecCols = BoardSize * BoardSize * 4;
			bool[,] ec = new bool[ecRows, ecCols];

			int index = 0;

			// Set the elements in the exact cover grid for the 4 constraints that there are
			// for a Sudoku game - these are:
			// Row-column, Row-number, Column-number, and Box-number

			// Set row-column constraints
			for (int r = 1; r <= BoardSize; r++)
			{
				for (int c = 1; c <= BoardSize; c++, index++)
				{
					for (int n = 1; n <= BoardSize; n++)
					{
						ec[Idx(r, c, n), index] = true;
					}
				}
			}

			// Set row-number constraints
			for (int r = 1; r <= BoardSize; r++)
			{
				for (int n = 1; n <= BoardSize; n++, index++)
				{
					for (int c1 = 1; c1 <= BoardSize; c1++)
					{
						ec[Idx(r, c1, n), index] = true;
					}
				}
			}

			// Set column-number constraints
			for (int c = 1; c <= BoardSize; c++)
			{
				for (int n = 1; n <= BoardSize; n++, index++)
				{
					for (int r1 = 1; r1 <= BoardSize; r1++)
					{
						ec[Idx(r1, c, n), index] = true;
					}
				}
			}

			// Set box-number constraints
			for (int br = 1; br <= BoardSize; br += BoxSize)
			{
				for (int bc = 1; bc <= BoardSize; bc += BoxSize)
				{
					for (int n = 1; n <= BoardSize; n++, index++)
					{
						for (int rDelta = 0; rDelta < BoxSize; rDelta++)
						{
							for (int cDelta = 0; cDelta < BoxSize; cDelta++)
							{
								ec[Idx(br + rDelta, bc + cDelta, n), index] = true;
							}
						}
					}
				}
			}

			// Modify the constraints set in the exact cover grid to account for the numbers in the
			// Sudoku puzzle that are already known.
			for (int row = 1; row <= BoardSize; row++)
			{
				for (int column = 1; column <= BoardSize; column++)
				{
					int n = sudoku[row - 1, column - 1];
					if (n != 0)
					{
						// zero out the constraint elements for this number
						for (int num = 1; num <= BoardSize; num++)
						{
							if (num != n)
							{
								int idx = Idx(row, column, num);
								for (int x = 0; x < ec.GetLength(1); x++)
								{
									ec[idx, x] = false;
								}
							}
						}
					}
				}
			}

			return ec;
		}
	}
}
