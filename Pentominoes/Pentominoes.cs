using System.Collections.Generic;
using System.Linq;

namespace PendleCodeMonkey.Pentominoes
{
	internal class Pentominoes
	{
		private const int ExcludedSquareID = -1;

		public Pentominoes()
		{
			BuildConfigurations();
		}

		internal List<PentominoConfiguration>? Configurations { get; set; }

		private int Rows { get; set; }
		private int Columns { get; set; }

		private int[,]? CoordinateToSquareNumberMap;

		private List<(int x, int y)>? SquareNumberToCoordinateMap;

		public static Dictionary<int, string> MapNumericIDToAlpha = new Dictionary<int, string>
		{
			{ 1, "F" },
			{ 2, "I" },
			{ 3, "L" },
			{ 4, "N" },
			{ 5, "P" },
			{ 6, "T" },
			{ 7, "U" },
			{ 8, "V" },
			{ 9, "W" },
			{ 10, "X" },
			{ 11, "Y" },
			{ 12, "Z" }
		};

		private void BuildConfigurations()
		{
			// All possible configurations (i.e. rotations and reflections) of the 12 pentominoes.
			// Each of the 12 pentominoes has a numerically assigned ID and an alphabetic ID (that
			// corresponds to the letter of the alphabet that the pentomino most resembles)
			Configurations = new List<PentominoConfiguration>()
			{
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (0, 0), (1, 0), (1, 1), (2, 1), (1, 2) }
				},
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (2, 0), (0, 1), (1, 1), (2, 1), (1, 2) }
				},
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (1, 0), (0, 1), (1, 1), (1, 2), (2, 2) }
				},
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (1, 0), (0, 1), (1, 1), (2, 1), (0, 2) }
				},
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (1, 0), (2, 0), (0, 1), (1, 1), (1, 2) }
				},
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (1, 0), (0, 1), (1, 1), (2, 1), (2, 2) }
				},
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (1, 0), (1, 1), (2, 1), (0, 2), (1, 2) }
				},
				new PentominoConfiguration { ID = 1, AlphaID = 'F',
					Coords = new() { (0, 0), (0, 1), (1, 1), (2, 1), (1, 2) }
				},
				new PentominoConfiguration { ID = 2, AlphaID = 'I',
					Coords = new() { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0) }
				},
				new PentominoConfiguration { ID = 2, AlphaID = 'I',
					Coords = new() { (0, 0), (0, 1), (0, 2), (0, 3), (0, 4) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (0, 0), (1, 0), (2, 0), (3, 0), (0, 1) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (0, 0), (1, 0), (1, 1), (1, 2), (1, 3) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (3, 0), (0, 1), (1, 1), (2, 1), (3, 1) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (0, 0), (0, 1), (0, 2), (0, 3), (1, 3) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (0, 0), (1, 0), (2, 0), (3, 0), (3, 1) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (1, 0), (1, 1), (1, 2), (0, 3), (1, 3) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (0, 0), (0, 1), (1, 1), (2, 1), (3, 1) }
				},
				new PentominoConfiguration { ID = 3, AlphaID = 'L',
					Coords = new() { (0, 0), (1, 0), (0, 1), (0, 2), (0, 3) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (0, 0), (1, 0), (2, 0), (2, 1), (3, 1) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (1, 0), (1, 1), (0, 2), (1, 2), (0, 3) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (0, 0), (1, 0), (1, 1), (2, 1), (3, 1) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (1, 0), (0, 1), (1, 1), (0, 2), (0, 3) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (1, 0), (2, 0), (3, 0), (0, 1), (1, 1) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (0, 0), (0, 1), (1, 1), (1, 2), (1, 3) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (2, 0), (3, 0), (0, 1), (1, 1), (2, 1) }
				},
				new PentominoConfiguration { ID = 4, AlphaID = 'N',
					Coords = new() { (0, 0), (0, 1), (0, 2), (1, 2), (1, 3) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (0, 0), (1, 0), (2, 0), (0, 1), (1, 1) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (0, 0), (1, 0), (0, 1), (1, 1), (1, 2) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (1, 0), (2, 0), (0, 1), (1, 1), (2, 1) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (0, 0), (0, 1), (1, 1), (0, 2), (1, 2) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (0, 0), (1, 0), (2, 0), (1, 1), (2, 1) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (1, 0), (0, 1), (1, 1), (0, 2), (1, 2) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (0, 0), (1, 0), (0, 1), (1, 1), (2, 1) }
				},
				new PentominoConfiguration { ID = 5, AlphaID = 'P',
					Coords = new() { (0, 0), (1, 0), (0, 1), (1, 1), (0, 2) }
				},
				new PentominoConfiguration { ID = 6, AlphaID = 'T',
					Coords = new() { (0, 0), (1, 0), (2, 0), (1, 1), (1, 2) }
				},
				new PentominoConfiguration { ID = 6, AlphaID = 'T',
					Coords = new() { (2, 0), (0, 1), (1, 1), (2, 1), (2, 2) }
				},
				new PentominoConfiguration { ID = 6, AlphaID = 'T',
					Coords = new() { (1, 0), (1, 1), (0, 2), (1, 2), (2, 2) }
				},
				new PentominoConfiguration { ID = 6, AlphaID = 'T',
					Coords = new() { (0, 0), (0, 1), (1, 1), (2, 1), (0, 2) }
				},
				new PentominoConfiguration { ID = 7, AlphaID = 'U',
					Coords = new() { (0, 0), (1, 0), (2, 0), (0, 1), (2, 1) }
				},
				new PentominoConfiguration { ID = 7, AlphaID = 'U',
					Coords = new() { (0, 0), (1, 0), (1, 1), (0, 2), (1, 2) }
				},
				new PentominoConfiguration { ID = 7, AlphaID = 'U',
					Coords = new() { (0, 0), (2, 0), (0, 1), (1, 1), (2, 1) }
				},
				new PentominoConfiguration { ID = 7, AlphaID = 'U',
					Coords = new() { (0, 0), (1, 0), (0, 1), (0, 2), (1, 2) }
				},
				new PentominoConfiguration { ID = 8, AlphaID = 'V',
					Coords = new() { (0, 0), (1, 0), (2, 0), (0, 1), (0, 2) }
				},
				new PentominoConfiguration { ID = 8, AlphaID = 'V',
					Coords = new() { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) }
				},
				new PentominoConfiguration { ID = 8, AlphaID = 'V',
					Coords = new() { (2, 0), (2, 1), (0, 2), (1, 2), (2, 2) }
				},
				new PentominoConfiguration { ID = 8, AlphaID = 'V',
					Coords = new() { (0, 0), (0, 1), (0, 2), (1, 2), (2, 2) }
				},
				new PentominoConfiguration { ID = 9, AlphaID = 'W',
					Coords = new() { (0, 0), (1, 0), (1, 1), (2, 1), (2, 2) }
				},
				new PentominoConfiguration { ID = 9, AlphaID = 'W',
					Coords = new() { (2, 0), (1, 1), (2, 1), (0, 2), (1, 2) }
				},
				new PentominoConfiguration { ID = 9, AlphaID = 'W',
					Coords = new() { (0, 0), (0, 1), (1, 1), (1, 2), (2, 2) }
				},
				new PentominoConfiguration { ID = 9, AlphaID = 'W',
					Coords = new() { (1, 0), (2, 0), (0, 1), (1, 1), (0, 2) }
				},
				new PentominoConfiguration { ID = 10, AlphaID = 'X',
					Coords = new() { (1, 0), (0, 1), (1, 1), (2, 1), (1, 2) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (0, 0), (1, 0), (2, 0), (3, 0), (1, 1) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (1, 0), (0, 1), (1, 1), (1, 2), (1, 3) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (2, 0), (0, 1), (1, 1), (2, 1), (3, 1) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (0, 0), (0, 1), (0, 2), (1, 2), (0, 3) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (0, 0), (1, 0), (2, 0), (3, 0), (2, 1) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (1, 0), (1, 1), (0, 2), (1, 2), (1, 3) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (1, 0), (0, 1), (1, 1), (2, 1), (3, 1) }
				},
				new PentominoConfiguration { ID = 11, AlphaID = 'Y',
					Coords = new() { (0, 0), (0, 1), (1, 1), (0, 2), (0, 3) }
				},
				new PentominoConfiguration { ID = 12, AlphaID = 'Z',
					Coords = new() { (0, 0), (1, 0), (1, 1), (1, 2), (2, 2) }
				},
				new PentominoConfiguration { ID = 12, AlphaID = 'Z',
					Coords = new() { (2, 0), (0, 1), (1, 1), (2, 1), (0, 2) }
				},
				new PentominoConfiguration { ID = 12, AlphaID = 'Z',
					Coords = new() { (1, 0), (2, 0), (1, 1), (0, 2), (1, 2) }
				},
				new PentominoConfiguration { ID = 12, AlphaID = 'Z',
					Coords = new() { (0, 0), (0, 1), (1, 1), (2, 1), (2, 2) }
				}
			};
		}

		// Generate the collections that map from coordinates to square numbers and vice-versa.
		internal void GenerateMaps(int numRows, int numColumns, List<(int x, int y)> excludedSquares)
		{
			Rows = numRows;
			Columns = numColumns;
			CoordinateToSquareNumberMap = new int[numColumns, numRows];
			SquareNumberToCoordinateMap = new();
			int sqrNum = 0;
			for (int y = 0; y < numRows; y++)
			{
				for (int x = 0; x < numColumns; x++)
				{
					if (excludedSquares.Contains((x, y)))
					{
						CoordinateToSquareNumberMap[x, y] = ExcludedSquareID;
					}
					else
					{
						CoordinateToSquareNumberMap[x, y] = sqrNum++;
						SquareNumberToCoordinateMap.Add((x, y));
					}
				}
			}
		}

		// Place each configuration of the requested set of pentominoes in all possible valid locations within the puzzle.
		// The pieceIDs parameter is a collection of ID of the pentominoes to be used (allowing a specific subset of the
		// 12 pentominoes to be used)
		internal List<(int id, List<int> squares)>? PlacePentominoes(IEnumerable<int> pieceIDs)
		{
			if (Configurations == null || CoordinateToSquareNumberMap == null)
			{
				return null;
			}

			List<(int id, List<int> squares)> validLocations = new();
			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					foreach (var config in Configurations)
					{
						if (!pieceIDs.Contains(config.ID))
						{
							continue;
						}
						// Check if the pentomino (in its current configuration) is valid for this location
						// (i.e. none of the pentomino falls outside the bounds of the puzzle and
						// none of it falls on an excluded square)
						List<int> squareNumbers = new();
						foreach (var (x, y) in config.Coords)
						{
							if (c + x >= Columns ||
								r + y >= Rows ||
								CoordinateToSquareNumberMap[c + x, r + y] == ExcludedSquareID)
							{
								break;
							}
							squareNumbers.Add(CoordinateToSquareNumberMap[c + x, r + y]);
						}

						// If we have a full set of 5 squares occupied on the puzzle then we have a valid
						// location at which this pentomino can be placed.
						if (squareNumbers.Count == 5)
						{
							validLocations.Add((config.ID, squareNumbers));
						}
					}
				}
			}

			return validLocations;
		}

		// Generate the exact cover matrix for a pentomino puzzle (so it can be solved using the AlgorithmX functionality)
		internal bool[,] MakeExactCoverMatrix(List<(int id, List<int> squares)> validPlacements, IEnumerable<int> pieceIDs)
		{
			Dictionary<int, int> mapPieceIDToExactCoverColumn = new();
			int colIndex = 0;
			foreach (var pieceID in pieceIDs)
			{
				mapPieceIDToExactCoverColumn.Add(pieceID, colIndex++);
			}

			bool[,] ec = new bool[validPlacements.Count, pieceIDs.Count() * 6];

			int row = 0;
			foreach (var placement in validPlacements)
			{
				ec[row, mapPieceIDToExactCoverColumn[placement.id]] = true;
				foreach (var square in placement.squares)
				{
					ec[row, square + pieceIDs.Count()] = true;
				}
				row++;
			}

			return ec;
		}

		internal int[,]? TranslateSolution(List<List<int>> soln, IEnumerable<int> pieceIDs)
		{
			if (CoordinateToSquareNumberMap == null)
			{
				return null;
			}

			int[] vals = new int[pieceIDs.Count() * 5];
			foreach (var sol in soln)
			{
				int pentominoID = pieceIDs.ElementAt(sol[0]);
				for (int i = 1; i <= 5; i++)
				{
					vals[sol[i] - pieceIDs.Count()] = pentominoID;
				}
			}

			int[,] newSoln = new int[Columns, Rows];
			for (int x = 0; x < Columns; x++)
			{
				for (int y = 0; y < Rows; y++)
				{
					int sqrNum = CoordinateToSquareNumberMap[x, y];
					if (sqrNum != ExcludedSquareID)
					{
						newSoln[x, y] = vals[sqrNum];
					}
				}
			}

			return newSoln;
		}
	}
}
