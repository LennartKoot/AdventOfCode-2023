namespace AdventOfCode;

public class Day_21 : BaseDay
{
    private readonly char[,] _grid;
    private readonly Position _start = new(-1, -1);

    record Position(int X, int Y);

    public Day_21()
    {
        var lines = File.ReadAllLines(InputFilePath);
        var max_y = lines.Length;
        var max_x = lines[0].Length;
        _grid = new char[lines.Length, lines[0].Length];

        for (int y = 0; y < max_y; y++)
            for (int x = 0; x < max_x; x++)
            {
                if (lines[y][x] == 'S')
                    _start = new(x, y);
                _grid[y, x] = lines[y][x];
            }
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solution_1()
    {
        HashSet<Position> reached = [_start];
        for (int i = 0; i < 64; i++)
        {
            HashSet<Position> newReached = [];
            foreach (var pos in reached)
            {
                if (GetValue(_grid, pos with { X = pos.X - 1}) != '#')
                    newReached.Add(pos with { X = pos.X - 1});
                if (GetValue(_grid, pos with { X = pos.X + 1}) != '#')
                    newReached.Add(pos with { X = pos.X + 1});
                if (GetValue(_grid, pos with { Y = pos.Y - 1}) != '#')
                    newReached.Add(pos with { Y = pos.Y - 1});
                if (GetValue(_grid, pos with { Y = pos.Y + 1}) != '#')
                    newReached.Add(pos with { Y = pos.Y + 1});
            }
            reached = newReached;
        }

        return reached.Count;
    }

    private static char GetValue(char[,] grid, Position position)
    {
        var max_x = grid.GetLength(1) - 1;
        var max_y = grid.GetLength(0) - 1;

        var (x, y) = position;
        if (x > max_x || y > max_y || x < 0 || y < 0)
            return '#';

        return grid[y, x];
    }
}
