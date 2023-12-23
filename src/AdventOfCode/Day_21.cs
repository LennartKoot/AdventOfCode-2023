using System.Text;

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

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

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

    public int Solution_2()
    {
        HashSet<Position> reached = [_start];
        for (int i = 0; i < 65; i++)
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

        /* Found pattern growing in a diamond shape, hitting the edges to new gardens in 66 steps
         * which is exactly half the width and height
         */
        PrintGrid(reached);
        return reached.Count;
    }

    private static char GetValue(char[,] grid, Position position, bool infiniteMap = false)
    {
        var max_x = grid.GetLength(1) - 1;
        var max_y = grid.GetLength(0) - 1;

        var (x, y) = position;
        if (x > max_x || y > max_y || x < 0 || y < 0) {
            if (!infiniteMap)
                return '#';

            if (x > max_x)
                x %= max_x + 1;
            if (y > max_y)
                y %= max_y + 1;
            if (x < 0)
                x = (x %= max_x + 1) < 0 ? x + max_x + 1: x;
            if (y < 0)
                y = (y %= max_y + 1) < 0 ? y + max_y + 1 : y;
        }

        return grid[y, x];
    }

    private void PrintGrid(HashSet<Position> reached) {
        var min_x = reached.MinBy(p => p.X).X;
        var min_y = reached.MinBy(p => p.Y).Y;
        var max_x = reached.MaxBy(p => p.X).X;
        var max_y = reached.MaxBy(p => p.Y).Y;

        if (min_x > 0)
            min_x = 0;
        if (min_y > 0)
            min_y = 0;

        var gridMaxX = _grid.GetLength(1) - 1;
        var gridMaxY = _grid.GetLength(0) - 1;
        if (max_x < gridMaxX)
            max_x = gridMaxX;
        if (max_y < gridMaxY)
            max_y = gridMaxY;

        var sb = new StringBuilder();
        for (int y = min_y; y <= max_y; y++) {
            for (int x = min_x; x <= max_x; x++) {
                var pos = new Position(x, y);
                if (reached.Contains(pos))
                    sb.Append('O');
                else {
                    var @char = GetValue(_grid, pos, true);
                    if (@char == 'S')
                        @char = '.';
                    sb.Append(@char);
                }
            }
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }
}
