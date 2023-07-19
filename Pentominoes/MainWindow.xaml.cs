using PendleCodeMonkey.AlgorithmXLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PendleCodeMonkey.Pentominoes
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Pentominoes _pentominoes;
		private readonly DataModel _model;

		private readonly string _appName = "PCMPentominoes";
		private readonly SettingsManager<UserSettings> _settingsManager;
		private readonly UserSettings? _userSettings;
		private readonly double _initialWindowHeight;
		private readonly double _initialWindowWidth;

		private PuzzleSquareState[,]? _puzzleLayout;

		private int _maxSolutions = 3000;
		private List<List<List<int>>> _solutions;
		private int _solutionNumber = 0;
		private int _puzzleIndex = 0;
		private bool _showAlphaLabels = false;
		private bool _solving = false;
		private CancellationTokenSource? _cancelSource;

		private List<(int solnStartIndex, IEnumerable<int> pieceIDs)> _pieceIDs = new();

		// The colours to be used for each of the 12 pentominoes.
		private readonly Color[] _pentColours = new Color[]
		{
			Color.FromRgb(0xdd, 0xbb, 0x99),
			Color.FromRgb(0xf6, 0xb2, 0xb1),
			Color.FromRgb(0xd2, 0xd3, 0x8e),
			Color.FromRgb(0xaa, 0xf1, 0xb0),
			Color.FromRgb(0xbf, 0xe2, 0x9f),
			Color.FromRgb(0x99, 0xe1, 0xc1),
			Color.FromRgb(0x87, 0xd1, 0xd2),
			Color.FromRgb(0x9d, 0xc1, 0xe2),
			Color.FromRgb(0xb1, 0xb0, 0xf2),
			Color.FromRgb(0xc4, 0xa0, 0xe2),
			Color.FromRgb(0xd7, 0x8f, 0xd3),
			Color.FromRgb(0xe6, 0xa1, 0xc2)
		};

		public static readonly RoutedCommand PreviousPuzzleCmd = new();
		public static readonly RoutedCommand NextPuzzleCmd = new();
		public static readonly RoutedCommand PreviousSolutionCmd = new();
		public static readonly RoutedCommand NextSolutionCmd = new();
		public static readonly RoutedCommand SolveCmd = new();
		public static readonly RoutedCommand AbortCmd = new();

		private enum PuzzleSquareState
		{
			Blank = 0,		// Blank square (for squares that are totally outside the puzzle)
			Piece = 1,		// Square that can be occupied by a pentomino piece
			Excluded = 2	// Excluded square (representing a hole inside the puzzle)
		}

		public MainWindow()
		{
			_pentominoes = new Pentominoes();
			_solutions = new();

			InitializeComponent();

			_model = new DataModel
			{
				MaxSolutions = 100
			};
			_userSettings = new UserSettings();
			_settingsManager = new SettingsManager<UserSettings>(_appName, "UserSettings.json");
			DataContext = _model;

			_initialWindowHeight = Height;
			_initialWindowWidth = Width;

			_userSettings = _settingsManager.LoadSettings() ?? new UserSettings();
			if (_userSettings != null)
			{
				UserSettings.LoadWindowStateSettings(_userSettings.MainWindowStateSettings, this);
			}
		}

		private void Solve()
		{
			if (_pentominoes != null)
			{
				var (rows, columns, excludedSquares, blankSquares) = GeneratePuzzle(Puzzles.PuzzleLayouts[_puzzleIndex]);
				excludedSquares = excludedSquares.Concat(blankSquares).ToList();
				_pentominoes.GenerateMaps(rows, columns, excludedSquares);


				_solutions = new();
				_pieceIDs = new();
				_solutionNumber = 0;
				if (MaxSolutionsCheckBox.IsChecked ?? false)
				{
					_maxSolutions = _model.MaxSolutions;
				}
				else
				{
					_maxSolutions = int.MaxValue;
				}

				_solving = true;
				progress.IsIndeterminate = true;
				StatusTextBlock.Text = (string)Application.Current.FindResource("statusSolving");
				_cancelSource = new CancellationTokenSource();
				Task.Factory.StartNew(() => DoSolve(rows, columns, excludedSquares, _cancelSource.Token))
				.ContinueWith(t =>
				{
					_solving = false;
					progress.IsIndeterminate = false;
					Dispatcher.Invoke(() =>
					{
						if (_solutions.Count == 0)
						{
							SolutionTextBlock.Text = (string)Application.Current.FindResource("noSolutions");
						}
						StatusTextBlock.Text = (string)Application.Current.FindResource("statusReady");
					});

					// Force CommandBinding's "CanExecute" to be re-evaluated (ensuring that the
					// enabled/disabled state of all buttons is updated upon completion)
					CommandManager.InvalidateRequerySuggested();
				}
				, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}

		private void DoSolve(int rows, int columns, List<(int x, int y)> excludedSquares, CancellationToken cancelToken)
		{
			// Determine the number of squares available for placing pentomino pieces into.
			int numSquares = (rows * columns) - (excludedSquares != null ? excludedSquares.Count : 0);

			// Determine all combinations of pentomino pieces that can make up the number of squares.
			// Each pentomino occupies 5 squares, so the number of pentominoes needed to fill numSquares is
			// numSquares/5.
			var combinations = Enumerable.Range(1, 12).Combinations(numSquares / 5);

			foreach (var combination in combinations)
			{
				var validLocations = _pentominoes.PlacePentominoes(combination);
				if (validLocations != null)
				{
					var exactCoverMatrix = _pentominoes.MakeExactCoverMatrix(validLocations, combination);

					int maxSol = _maxSolutions - _solutions.Count;
					if (maxSol <= 0)
					{
						// We've reached the maximum number of solutions so bail out now.
						return;
					}

					// Keep a record of the list of piece IDs associated with the solutions that we are about to generate
					// (specifically for cases where we have a puzzle that does not need all 12 pentominoes).
					_pieceIDs.Add((_solutions.Count, combination));

					AlgoX dlx = new(exactCoverMatrix, HandleSolution, AlgoX.ColumnNaming.Numeric);

					dlx.Solve(maxSol, cancelToken);
					if (cancelToken.IsCancellationRequested)
					{
						return;
					}
				}
			}
		}

		// Generate a puzzle from its string representation (see Puzzles.cs for the list of these strings)
		private (int rows, int columns, List<(int x, int y)> excludedSquares, List<(int x, int y)> blankSquares) GeneratePuzzle(List<string> puzzle)
		{
			int rows = puzzle.Count;
			int columns = puzzle[0].Length;

			_puzzleLayout = new PuzzleSquareState[rows, columns];

			List<(int x, int y)> excludedSquares = new();
			List<(int x, int y)> blankSquares = new();
			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < puzzle[y].Length; x++)
				{
					if (puzzle[y][x] == '0')
					{
						excludedSquares.Add((x, y));
						_puzzleLayout[y, x] = PuzzleSquareState.Excluded;
					}
					else if (puzzle[y][x] == '-')
					{
						blankSquares.Add((x, y));
						_puzzleLayout[y, x] = PuzzleSquareState.Blank;
					}
					else
					{
						_puzzleLayout[y, x] = PuzzleSquareState.Piece;
					}
				}
			}

			return (rows, columns, excludedSquares, blankSquares);
		}

		private void HandleSolution(List<Node> solution)
		{
			List<List<int>> sol = new();
			foreach (Node n in solution)
			{
				List<int> solnValues = new();
				int val = int.Parse(n.ColumnHeader!.Name);
				solnValues.Add(val);
				Node tmp = n.Right;
				while (tmp != n)
				{
					val = int.Parse(tmp.ColumnHeader!.Name);
					solnValues.Add(val);
					tmp = tmp.Right;
				}
				sol.Add(solnValues.OrderBy(x => x).ToList());
			}

			// Add this solution to the collection.
			_solutions.Add(sol);

			Dispatcher.Invoke(() =>
			{
				if (_solutions.Count == 1)
				{
					_solutionNumber = 0;
					ShowSolution(_solutionNumber);
				}
				else
				{
					if (_solutions.Count == 2)
					{
						// Force CommandBinding's "CanExecute" to be re-evaluated (ensuring that the
						// enabled/disabled state of all buttons is updated upon completion)
						CommandManager.InvalidateRequerySuggested();
					}
					SolutionTextBlock.Text = string.Format((string)Application.Current.FindResource("solutionNumberFormat"), _solutionNumber + 1, _solutions.Count);
				}
			});
		}

		// Display the layout of the specified (unsolved) puzzle.
		private void ShowEmptyPuzzle(int puzzleIndex)
		{
			var (rows, columns, excludedSquares, blankSquares) = GeneratePuzzle(Puzzles.PuzzleLayouts[puzzleIndex]);

			board.Children.Clear();
			int size = Math.Max(rows, columns);
			board.Rows = size;
			board.Columns = size;
			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < columns; x++)
				{
					if (excludedSquares.Contains((x, y)))
					{
						ExcludedSquareControl excludedSqr = new()
						{
							NeighbouringIDs = (0, 0, 0, 0)
						};
						board.Children.Add(excludedSqr);
					}
					else if (blankSquares.Contains((x, y)))
					{
						BlankSquareControl blankSqr = new();
						board.Children.Add(blankSqr);
					}
					else
					{
						EmptySquareControl emptySqr = new();
						board.Children.Add(emptySqr);
					}
				}
				for (int i = 0; i < size - columns; i++)
				{
					BlankSquareControl blankSqr = new();
					board.Children.Add(blankSqr);
				}
			}
			PageTextBlock.Text = string.Format((string)Application.Current.FindResource("puzzleNumberFormat"), puzzleIndex + 1, Puzzles.PuzzleLayouts.Count);

			_solutionNumber = 0;
			_solutions = new();
			SolutionTextBlock.Text = (string)Application.Current.FindResource("notYetSolved");
		}

		// Display a specified solution to the current puzzle.
		private void ShowSolution(int solnIndex)
		{
			if (solnIndex < 0 || solnIndex >= _solutions.Count || _puzzleLayout == null)
			{
				return;
			}

			IEnumerable<int>? pieceIDs = null;
			for (int i = 0; i < _pieceIDs.Count - 1; i++)
			{
				if (solnIndex < _pieceIDs[i + 1].solnStartIndex)
				{
					pieceIDs = _pieceIDs[i].pieceIDs;
					break;
				}
			}
			pieceIDs ??= _pieceIDs.Last().pieceIDs;

			var solnIDs = _pentominoes.TranslateSolution(_solutions[solnIndex], pieceIDs);

			if (solnIDs != null)
			{
				board.Children.Clear();
				int rows = solnIDs.GetLength(1);
				int columns = solnIDs.GetLength(0);
				int size = Math.Max(rows, columns);
				board.Rows = size;
				board.Columns = size;
				for (int y = 0; y < rows; y++)
				{
					for (int x = 0; x < columns; x++)
					{
						int north = y > 0 ? solnIDs[x, y - 1] : -1;
						int south = y < rows - 1 ? solnIDs[x, y + 1] : -1;
						int west = x > 0 ? solnIDs[x - 1, y] : -1;
						int east = x < columns - 1 ? solnIDs[x + 1, y] : -1;

						switch (_puzzleLayout[y, x])
						{
							case PuzzleSquareState.Excluded:
								ExcludedSquareControl excludedSqr = new()
								{
									NeighbouringIDs = (north, east, south, west)
								};
								board.Children.Add(excludedSqr);
								break;
							case PuzzleSquareState.Blank:
								BlankSquareControl blankSqr = new();
								board.Children.Add(blankSqr);
								break;
							default:
								Color col = _pentColours[solnIDs[x, y] - 1];
								OccupiedSquareControl sqr = new()
								{
									PentominoID = solnIDs[x, y],
									NeighbouringIDs = (north, east, south, west),
									PentominoColour = col,
									ShowAlphaLabel = _showAlphaLabels
								};
								board.Children.Add(sqr);
								break;
						}
					}
					for (int i = 0; i < size - columns; i++)
					{
						BlankSquareControl blankSqr = new();
						board.Children.Add(blankSqr);
					}
				}
			}
			SolutionTextBlock.Text = string.Format((string)Application.Current.FindResource("solutionNumberFormat"), solnIndex + 1, _solutions.Count);

		}

		// ExecutedRoutedEventHandler for the Previous Puzzle command.
		private void PreviousPuzzleCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_puzzleIndex--;
			ShowEmptyPuzzle(_puzzleIndex);
		}

		// CanExecuteRoutedEventHandler for the Previous Puzzle command.
		private void PreviousPuzzleCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !_solving && _puzzleIndex > 0;
		}

		// ExecutedRoutedEventHandler for the Next Puzzle command.
		private void NextPuzzleCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_puzzleIndex++;
			ShowEmptyPuzzle(_puzzleIndex);
		}

		// CanExecuteRoutedEventHandler for the Next Puzzle command.
		private void NextPuzzleCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !_solving && _puzzleIndex < Puzzles.PuzzleLayouts.Count - 1;
		}


		// ExecutedRoutedEventHandler for the Previous Puzzle command.
		private void PreviousSolutionCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_solutionNumber--;
			ShowSolution(_solutionNumber);
		}

		// CanExecuteRoutedEventHandler for the Previous Puzzle command.
		private void PreviousSolutionCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _solutionNumber > 0;
		}

		// ExecutedRoutedEventHandler for the Next Puzzle command.
		private void NextSolutionCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_solutionNumber++;
			ShowSolution(_solutionNumber);
		}

		// CanExecuteRoutedEventHandler for the Next Puzzle command.
		private void NextSolutionCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _solutionNumber < _solutions.Count - 1;
		}

		// ExecutedRoutedEventHandler for the Next Puzzle command.
		private void SolveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Solve();
		}

		// CanExecuteRoutedEventHandler for the Next Puzzle command.
		private void SolveCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			var maxApplied = MaxSolutionsCheckBox.IsChecked ?? false;
			e.CanExecute = !_solving && (!maxApplied || IsValid(this));
		}

		// ExecutedRoutedEventHandler for the Abort command.
		private void AbortCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_cancelSource?.Cancel();
		}

		// CanExecuteRoutedEventHandler for the Abort command.
		private void AbortCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _solving;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Don't want to allow the window to be resized any smaller than the initial (default) size.
			SetValue(MinWidthProperty, _initialWindowWidth);
			SetValue(MinHeightProperty, _initialWindowHeight);

			if (_userSettings != null)
			{
				_showAlphaLabels = _userSettings.AlphaLabelEnabled;
				ShowAlphaLabelsCheckBox.IsChecked = _showAlphaLabels;
				MaxSolutionsCheckBox.IsChecked = _userSettings.MaxSolutionsEnabled;
				_model.MaxSolutions = _userSettings.MaxSolutions;
			}

			// Initially display the first (unsolved) puzzle.
			ShowEmptyPuzzle(_puzzleIndex);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_userSettings != null)
			{
				_userSettings.AlphaLabelEnabled = _showAlphaLabels;
				_userSettings.MaxSolutionsEnabled = MaxSolutionsCheckBox.IsChecked ?? false;
				_userSettings.MaxSolutions = _model.MaxSolutions;

				_userSettings.MainWindowStateSettings = UserSettings.GetWindowStateSettings(this);
				_settingsManager.SaveSettings(_userSettings);
			}
		}

		private void ShowAlphaLabelsCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			_showAlphaLabels = true;
			if (_solutions.Count > 0)
			{
				ShowSolution(_solutionNumber);
			}
		}

		private void ShowAlphaLabelsCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			_showAlphaLabels = false;
			if (_solutions.Count > 0)
			{
				ShowSolution(_solutionNumber);
			}
		}

		private bool IsValid(DependencyObject obj)
		{
			// The dependency object is valid if it has no errors and all
			// of its children (that are dependency objects) are error-free.
			return !Validation.GetHasError(obj) && LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>().All(IsValid);
		}
	}
}
