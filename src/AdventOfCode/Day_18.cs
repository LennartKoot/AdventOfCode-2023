namespace AdventOfCode;

public class Day_18 : BaseDay
{
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    record Coordinate(int X, int Y);

    public int Solution_1() {
        static (char, int) fGetDirAndLength(string line)
        {
            var splitted = line.Split(' ');
            var direction = splitted[0][0];
            var length = int.Parse(splitted[1]);
            return (direction, length);
        }

        var polygon = CreatePolygon(File.ReadLines(InputFilePath), fGetDirAndLength);
        var ((min_x, min_y), (max_x, max_y)) = polygon.BoundingBox;
        var grid = new bool[max_y - min_y + 1, max_x - min_x + 1];
        for (int i = 0; i < polygon.Points.Count; i++) {
            var (from_x, from_y) = polygon.Points[i];
            var (to_x, to_y) = polygon.Points[(i + 1) % polygon.Points.Count];
            if (from_x > to_x)
                Swap(ref from_x, ref to_x);
            if (from_y > to_y)
                Swap(ref from_y, ref to_y);
            if (from_x == to_x) {
                for (int y = from_y - min_y; y <= to_y - min_y; y++)
                    grid[y, from_x - min_x] = true;
            }
            else {
                for (int x = from_x - min_x; x <= to_x - min_x; x++)
                    grid[from_y - min_y, x] = true;
            }
        }

        var rows = grid.GetLength(0);
        var columns = grid.GetLength(1);

        var area = 0;
        for (int y = 0; y < rows; y++) {
            bool? wasPreviousCrossUp = null;
            bool inside = false;
            for (int x = 0; x < columns; x++) {
                if (grid[y, x]) {
                    var up = GetValue(grid, x, y - 1);
                    var down = GetValue(grid, x, y - 1);
                    if (!up && !down) {
                        ++area;
                        continue;
                    }

                    if ( // Similar to Day 10
                        wasPreviousCrossUp == null ||
                        wasPreviousCrossUp.Value && down ||
                        !wasPreviousCrossUp.Value && up
                    ) {
                        inside = !inside;
                        wasPreviousCrossUp = up;
                    }
                }
                if (inside || grid[y, x])
                    ++area;
            }
        }

        return area;
    }

    record BoundingBox(Coordinate Min, Coordinate Max);
    record Polygon(IList<Coordinate> Points, BoundingBox BoundingBox);

    private static Polygon CreatePolygon(IEnumerable<string> lines, Func<string, (char, int)> fGetDirAndLength) {
        var points = new List<Coordinate>();

        var previousCorner = new Coordinate(0,0); // Not added to polygon yet, will happen at last instruction.
        int min_x = int.MaxValue, min_y = int.MaxValue, max_x = 0, max_y = 0;

        foreach (var instruction in lines) {
            var (direction, length) = fGetDirAndLength(instruction);
            var nextCorner = direction switch {
                'U' => previousCorner with { Y = previousCorner.Y - length },
                'R' => previousCorner with { X = previousCorner.X + length },
                'D' => previousCorner with { Y = previousCorner.Y + length },
                'L' => previousCorner with { X = previousCorner.X - length },
                _ => throw new SolvingException(),
            };
            points.Add(nextCorner);
            previousCorner = nextCorner;
            min_x = nextCorner.X < min_x ? nextCorner.X : min_x;
            min_y = nextCorner.Y < min_y ? nextCorner.Y : min_y;
            max_x = nextCorner.X > max_x ? nextCorner.X : max_x;
            max_y = nextCorner.Y > max_y ? nextCorner.Y : max_y;
        }

        return new(points, new(new(min_x, min_y), new(max_x, max_y)));
    }

    private static void Swap(ref int a, ref int b) {
        a += b;
        b = a - b;
        a -= b;
    }

    private static bool GetValue(bool[,] grid, int x, int y) {
        var rows = grid.GetLength(0);
        var columns = grid.GetLength(1);
        if (x < 0 || y < 0 || x >= columns || y >= rows)
            return false;
        return grid[y, x];
    }
}
