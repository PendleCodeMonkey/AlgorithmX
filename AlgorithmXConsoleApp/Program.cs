using PendleCodeMonkey.AlgorithmXConsoleApp;
using static PendleCodeMonkey.AlgorithmXLib.AlgoX;


string[] exampleSudokus = new string[]
{
	"4-----3-----8-2------7--------1---8734-------6--------5---6--------1-4---82------",
	"1-----3-8-6-4--------------2-3-1-----------958---------5-6---7-----8-2---4-------",
	"-834---------7--5-----------4-1-8----------27---3-----2-6-5----5-----8--------1--",
	"---57--3-1------2-7---234------8---4--7--4---49----6-5-42---3-----7--9----18-----",
	"-----8--3-16-2-9-7-3---46-----------9-5---2---2-13---9--3----2--7---5---------4--"
};

Sudoku sudoku = new();

foreach (string sudokuStr in exampleSudokus)
{
	// Construct the 9x9 array of values in the sudoku board
	int[,] data = new int[9, 9];
	int index = 0;
	foreach (var c in sudokuStr)
	{
		data[index / 9, index % 9] = c >= '1' && c <= '9' ? c - '0' : 0;
		index++;
	}
	// Attempt to get a solution to this sudoku puzzle.
	// The first solution found is output to the console window.
	sudoku.Solve(data, 1, ColumnNaming.Numeric);
	Console.WriteLine();
}

Console.WriteLine("Press any key");
Console.ReadKey();
