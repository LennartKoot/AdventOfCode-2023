using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode;

public class Day_10 : BaseDay
{
    private readonly char[][] _input;

    public Day_10()
    {
        _input = File.ReadLines(InputFilePath).Select(s => s.ToCharArray()).ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solution_1() {
        (int, int) start = (-1, -1);
        var max_x = _input[0].Length;
        var max_y = _input.Length;
        for (int y = 0; y < max_y; y++)
            for (int x = 0; x < max_x; x++)
                if (_input[y][x] == 'S')
                    start = (x, y); // TODO: Find start while parsing input

        var loop = FindLoop(_input, start);
        return loop.Count / 2;
    }

    enum Direction {
        North,
        East,
        South,
        West,
    }

    private static List<(int, int, Direction)> FindLoop(char[][] input, (int, int) start) {
        var (startPipe, startDirection) = DetermineStartPipe(input, start);
        var (startX, startY) = start;
        input[startY][startX] = startPipe;

        List<(int, int, Direction)> loop = [(startX, startY, startDirection)];

        var startFound = false;
        while (!startFound) {
            AddNextNode(loop[^1]);
            var (x, y, _) = loop[^1];
            startFound = (x, y) == start;
        }

        void AddNextNode((int, int, Direction) current) {
            var (x, y, nextDirection) = current;
            switch (nextDirection) {
                case Direction.North:
                    loop.Add((x, y - 1, NextDirection(input[y - 1][x], Direction.South)));
                    break;
                case Direction.South:
                    loop.Add((x, y + 1, NextDirection(input[y + 1][x], Direction.North)));
                    break;
                case Direction.East:
                    loop.Add((x + 1, y, NextDirection(input[y][x + 1], Direction.West)));
                    break;
                case Direction.West:
                    loop.Add((x - 1, y, NextDirection(input[y][x - 1], Direction.East)));
                    break;
            }
        }

        return loop;
    }

    private static (char, Direction) DetermineStartPipe(char[][] input, (int, int) start) {
        var (x, y) = start;
        var max_x = input[0].Length;
        var max_y = input.Length;

        if (x == 0 && y == 0)
            return ('F', Direction.South);
        if (x == max_x - 1 && y == max_y - 1)
            return ('J', Direction.West);
        if (x == 0 && y == max_y - 1)
            return ('L', Direction.North);
        if (x == max_x - 1 && y == 0)
            return ('7', Direction.East);

        bool north = false, south = false, east = false, west = false;

        if ((y - 1) > 0) {
            var pipe = input[y - 1][x];
            north = pipe == '|' || pipe == '7' || pipe == 'F'; 
        }
        if ((y + 1) < max_y) {
            var pipe = input[y + 1][x];
            south = pipe == '|' || pipe == 'L' || pipe == 'J';
        }
        if ((x - 1) > 0) {
            var pipe = input[y][x - 1];
            west = pipe == '-' || pipe == 'L' || pipe == 'F';
        }
        if ((x + 1) < max_x) {
            var pipe = input[y][x + 1];
            east = pipe == '-' || pipe == 'J' || pipe == '7';
        }

        return (north, south, east, west) switch {
            (true, true, _, _) => ('|', Direction.North),
            (_, _, true, true) => ('-', Direction.East),
            (true, _, true, _) => ('L', Direction.East),
            (true, _, _, true) => ('J', Direction.West),
            (_, true, _, true) => ('7', Direction.West),
            (_, true, true, _) => ('F', Direction.South),
            _ => throw new SolvingException(),
        };
    }

    private static Direction NextDirection(char pipe, Direction from) {
        return (pipe, from) switch {
            ('|', Direction.North) => Direction.South,
            ('|', Direction.South) => Direction.North,
            ('-', Direction.East) => Direction.West,
            ('-', Direction.West) => Direction.East,
            ('L', Direction.North) => Direction.East,
            ('L', Direction.East) => Direction.North,
            ('J', Direction.North) => Direction.West,
            ('J', Direction.West) => Direction.North,
            ('7', Direction.South) => Direction.West,
            ('7', Direction.West) => Direction.South,
            ('F', Direction.South) => Direction.East,
            ('F', Direction.East) => Direction.South,
            _ => throw new SolvingException(),
        };
    }
}
