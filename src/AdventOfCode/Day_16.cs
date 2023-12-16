using System.Collections.Concurrent;

namespace AdventOfCode;

#nullable enable

public class Day_16 : BaseDay
{
    private readonly char[][] _input;

    public Day_16()
    {
        _input = File
            .ReadLines(InputFilePath)
            .Select(line => line.ToCharArray())
            .ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    enum Direction : int {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
    }

    record Position(int X, int Y);

    public int Solution_1() {
        var visited = new HashSet<(Position, Direction)>();
        var energized = new HashSet<Position>();

        var startPosition = new Position(0, 0);
        var startDirection = Direction.East;

        MoveBeam(_input, visited, energized, startPosition, startDirection);

        return energized.Count;
    }

    public int Solution_2() {
        var max_x = _input[0].Length;
        var max_y = _input.Length;

        var startPositions = new List<Position>();
        for (int x = 0; x < max_x; x++) {
            startPositions.Add(new(x, 0));
            startPositions.Add(new(x, max_y - 1));
        }

        for (int y = 1; y < max_y - 1; y++) {
            startPositions.Add(new(0, y));
            startPositions.Add(new(max_x - 1, y));
        }

        var results = new ConcurrentBag<int>();
        Parallel.ForEach(startPositions, start => {
            foreach (var direction in new Direction[] { Direction.North, Direction.East, Direction.South, Direction.West }) {
                var visited = new HashSet<(Position, Direction)>();
                var energized = new HashSet<Position>();
                var grid = _input.Select(a => a.ToArray()).ToArray();

                MoveBeam(grid, visited, energized, start, direction);
                results.Add(energized.Count);
            }
        });

        return results.Max();
    }

    private static void MoveBeam(char[][] grid, HashSet<(Position, Direction)> visited, HashSet<Position> energized,
        Position start, Direction direction
    ) {
        if (!visited.Add((start, direction)))
            return; // Already visited this position with this direction.

        energized.Add(start);

        var currentSpace = grid[start.Y][start.X];
        if (currentSpace == '.') {
            NewMove(direction);
        }
        if (currentSpace == '-') {
            if (direction == Direction.East || direction == Direction.West)
                NewMove(direction);
            else {
                NewMove(Direction.East);
                NewMove(Direction.West);
            }
        }
        if (currentSpace == '|') {
            if (direction == Direction.North || direction == Direction.South)
                NewMove(direction);
            else {
                NewMove(Direction.North);
                NewMove(Direction.South);
            }
        }
        if (currentSpace == '/') {
            var newDirection = direction switch {
                Direction.North => Direction.East,
                Direction.East  => Direction.North,
                Direction.South => Direction.West,
                Direction.West  => Direction.South,
                _ => throw new SolvingException(),
            };
            NewMove(newDirection);
        }
        if (currentSpace == '\\') {
            var newDirection = direction switch {
                Direction.North => Direction.West,
                Direction.West  => Direction.North,
                Direction.South => Direction.East,
                Direction.East  => Direction.South,
                _ => throw new SolvingException(),
            };
            NewMove(newDirection);
        }

        void NewMove(Direction newDirection) {
            var newPosition = MoveBeamAcrossEmptySpace(grid, energized, start, newDirection);
            if (newPosition != null)
                MoveBeam(grid, visited, energized, newPosition, newDirection);
        }
    }

    /// <returns>The <see cref="Position"/> where the beam encountered the next mirror or splitter or null if edge.</returns>
    private static Position? MoveBeamAcrossEmptySpace(char[][] grid, HashSet<Position> energized, Position start, Direction direction) {
        Func<Position, Position> updatePosition = direction switch {
            Direction.North => pos => pos with { Y = pos.Y - 1 },
            Direction.East  => pos => pos with { X = pos.X + 1 },
            Direction.South => pos => pos with { Y = pos.Y + 1 },
            Direction.West  => pos => pos with { X = pos.X - 1 },
            _ => throw new SolvingException(),
        };

        var max_x = grid[0].Length;
        var max_y = grid.Length;

        var position = updatePosition(start);
        if (position.X >= max_x || position.Y >= max_y || position.X < 0 || position.Y < 0)
                return null;

        while (grid[position.Y][position.X] == '.') {
            energized.Add(position);
            position = updatePosition(position);
            if (position.X >= max_x || position.Y >= max_y || position.X < 0 || position.Y < 0)
                return null;
        }

        return position;
    }
}
