using System.Diagnostics;
using Microsoft.VisualBasic;

namespace AdventOfCode;

public class Day_17 : BaseDay
{
    private readonly int[][] _grid;

    public Day_17()
    {
        _grid = File
            .ReadLines(InputFilePath)
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solution_1() {
        // Direction does not matter. Steps and HeatLoss must start at 0.
        var source = new Vertex(0, 0, 0, Direction.East, 0);
        // HeatLoss, Direction and StepsInDirection do not matter.
        var target = new Vertex(_grid[0].Length - 1, _grid.Length - 1, 0, Direction.East, 0);

        IEnumerable<Vertex> fGetNeighbours(Vertex vertex) => GetNeighbours(vertex, 3);
        var (dist, prev) = Dijkstra(fGetNeighbours, source, target);

        var path = GetShortestPath(prev, source, target);
        return path.Sum(v => v.HeatLoss);
    }

    enum Direction {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
    }

    [DebuggerDisplay("({X}, {Y}) -> {StepsInDirection} {Direction} | {HeatLoss}")]
    record Vertex(int X, int Y, int HeatLoss, Direction Direction, int StepsInDirection);

    private IEnumerable<Vertex> GetNeighbours(Vertex source, int maxStepsInDirection) {
        var directions = new Direction[] { source.Direction, (Direction)(((int)source.Direction + 1) % 4), (Direction)(((int)source.Direction + 3) % 4) };
        foreach (var direction in directions) {
            var canMove = TryMove(source, direction, out var target);
            if (canMove && target.StepsInDirection <= maxStepsInDirection)
                yield return target;
        }
    }

    private bool TryMove(Vertex source, Direction direction, out Vertex target) {
        var newSteps = source.Direction == direction
            ? source.StepsInDirection + 1
            : 1;

        var max_x = _grid[0].Length - 1;
        var max_y = _grid.Length - 1;

        target = null;

        switch (direction) {
            case Direction.North:
                if (source.Y == 0)
                    return false;
                target = source with {
                    Y = source.Y - 1,
                    Direction = direction,
                    StepsInDirection = newSteps,
                    HeatLoss = _grid[source.Y - 1][source.X],
                };
                break;
            case Direction.East:
                if (source.X == max_x)
                    return false;
                target = source with {
                    X = source.X + 1,
                    Direction = direction,
                    StepsInDirection = newSteps,
                    HeatLoss = _grid[source.Y][source.X + 1],
                };
                break;
            case Direction.South:
                if (source.Y == max_y)
                    return false;
                target = source with {
                    Y = source.Y + 1,
                    Direction = direction,
                    StepsInDirection = newSteps,
                    HeatLoss = _grid[source.Y + 1][source.X],
                };
                break;
            case Direction.West:
                if (source.X == 0)
                    return false;
                target = source with {
                    X = source.X - 1,
                    Direction = direction,
                    StepsInDirection = newSteps,
                    HeatLoss = _grid[source.Y][source.X - 1],
                };
                break;
            default:
                throw new SolvingException();
        }

        return true;
    }

    private static (Dictionary<Vertex, int>, Dictionary<Vertex, Vertex>) Dijkstra(Func<Vertex,IEnumerable<Vertex>> fGetNeighbours, Vertex source, Vertex target) {
        var dist = new Dictionary<Vertex, int>();
        var prev = new Dictionary<Vertex,Vertex>();
        dist.Add(source, 0);

        var Q = new PriorityQueue<Vertex, int>();
        Q.Enqueue(source, 0);

        while (Q.Count != 0) {
            Q.TryDequeue(out var u, out var p);
            if (u.X == target.X && u.Y == target.Y)
                break;
            if (p != dist[u])
                continue;

            var neighbours = fGetNeighbours(u);
            foreach(var v in neighbours) {
                var alt = dist[u] + v.HeatLoss;

                var distV = int.MaxValue;
                if (dist.TryGetValue(v, out var value))
                    distV = value;

                if (alt < distV) {
                    dist[v] = alt;
                    if (!prev.TryAdd(v, u))
                        prev[v] = u;
                    Q.Enqueue(v, alt);
                }
            }
        }

        return (dist, prev);
    }

    private static IList<Vertex> GetShortestPath(Dictionary<Vertex, Vertex> prev, Vertex source, Vertex target) {
        var S = new Stack<Vertex>();
        var u = prev.First(kvp => kvp.Key.X == target.X && kvp.Key.Y == target.Y).Key;

        S.Push(u);
        while (prev.TryGetValue(u, out u)) {
            S.Push(u);
        }
        return S.ToList();
    }
}
