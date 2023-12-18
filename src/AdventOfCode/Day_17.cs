using System.Diagnostics;

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

    public override ValueTask<string> Solve_1() => new($"{Solve(1, 3)}");

    public override ValueTask<string> Solve_2() => new($"{Solve(4, 10)}");

    public int Solve(int minSteps, int maxSteps) {
        // Direction does not matter. Steps and HeatLoss must start at 0.
        var source = new Vertex(0, 0, 0, Direction.East, 0);
        // HeatLoss, Direction and StepsInDirection do not matter.
        var target = new Vertex(_grid[0].Length - 1, _grid.Length - 1, 0, Direction.East, 0);

        IEnumerable<Vertex> fGetNeighbours(Vertex vertex) => GetNeighbours(vertex, minSteps, maxSteps);
        var (dist, prev) = Dijkstra(fGetNeighbours, source);

        var path = GetShortestPath(prev, dist, target);
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

    private IEnumerable<Vertex> GetNeighbours(Vertex source, int minStepsInDirection, int maxStepsInDirection) {
        var directions = new Direction[] { source.Direction, (Direction)(((int)source.Direction + 1) % 4), (Direction)(((int)source.Direction + 3) % 4) };
        foreach (var direction in directions) {
            var canMove = TryMove(source, direction, out var target);
            if (canMove) {
                if (target.StepsInDirection >= minStepsInDirection && target.StepsInDirection <= maxStepsInDirection)
                    yield return target;
                else if (target.StepsInDirection < minStepsInDirection) {
                    while (TryMove(target, direction, out var tempTarget) && tempTarget.StepsInDirection <= minStepsInDirection)
                        target = tempTarget with { HeatLoss = target.HeatLoss + tempTarget.HeatLoss };
                    if (target.StepsInDirection == minStepsInDirection)
                        yield return target;
                }
            }
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

    private static (Dictionary<Vertex, int>, Dictionary<Vertex, Vertex>) Dijkstra(Func<Vertex,IEnumerable<Vertex>> fGetNeighbours, Vertex source) {
        var dist = new Dictionary<Vertex, int>();
        var prev = new Dictionary<Vertex,Vertex>();
        dist.Add(source, 0);

        var Q = new PriorityQueue<Vertex, int>();
        Q.Enqueue(source, 0);

        while (Q.Count != 0) {
            Q.TryDequeue(out var u, out var p);
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

    private static IList<Vertex> GetShortestPath(Dictionary<Vertex, Vertex> prev, Dictionary<Vertex, int> dist, Vertex target) {
        var S = new Stack<Vertex>();
        var targets = prev.Where(kvp => kvp.Key.X == target.X && kvp.Key.Y == target.Y).Select(kvp => kvp.Key);
        var u = targets.Aggregate((v1, v2) => dist[v1] < dist[v2] ? v1 : v2);

        S.Push(u);
        while (prev.TryGetValue(u, out u)) {
            S.Push(u);
        }
        return S.ToList();
    }
}
