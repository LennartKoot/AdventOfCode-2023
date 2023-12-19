using System.Globalization;

namespace AdventOfCode;

public class Day_18 : BaseDay
{
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    record Coordinate(int X, int Y);

    public long Solution_1() {
        static (char, int) fGetDirAndLength(string line)
        {
            var splitted = line.Split(' ');
            var direction = splitted[0][0];
            var length = int.Parse(splitted[1]);
            return (direction, length);
        }
        return Solve(fGetDirAndLength);
    }

    public long Solution_2() {
        static (char, int) fGetDirAndLength(string line) {
            var splitted = line.Split(' ');
            var hex = splitted[2][2..^1];
            var length = int.Parse(hex[..5], NumberStyles.HexNumber);
            var direction = hex[^1] switch {
                '0' => 'R',
                '1' => 'D',
                '2' => 'L',
                '3' => 'U',
                _ => throw new SolvingException(),
            };
            return (direction, length);
        }
        List<Coordinate> test = [new(0,0), new(3,0), new (3,3), new (0, 3)];
        Console.WriteLine($"{CalculateAreaOfSimplePolygon(test)} {Perimeter(test)}");
        return Solve(fGetDirAndLength);
    }

    private long Solve(Func<string, (char, int)> fGetDirAndLength) {
        var polygon = CreatePolygon(File.ReadLines(InputFilePath), fGetDirAndLength);
        var A = CalculateAreaOfSimplePolygon(polygon.Points);
        var P = Perimeter(polygon.Points);

        /* Let's take a simple polygon of 4x4 square for the trench. Coordinates will be (0,0) (3,0), (3,3), (0, 3)
         * ####
         * #..#
         * #..#
         * ####
         * We calculate A as being 9, meaning it is a 3x3 square. If you put this 3x3 square in the middle of the above diagram, 
         * you need to take part of the area of the trench. For cells where the trench is moving straight, this is 1/2 of the area.
         * For all corners in the example, you lose 1/4. We divide P by 2 averaging each cell to missing 1/2, so we need to make up for the corners.
         * All corners need to be accounted for with 3/4 of the cell. We only accounted for 1/2, so we need to add 4 * 1/4.
         * In more complex polygons you will also have concave corners losing more space (3/4), so we should substract 1/4 for every such corner.
         * Since convex and concave corners average each other out to the 1/2 loss we alreadye accounted for, we're only interested in the difference between these two types of corners.
         * To close the loop of the polygon, you will always have 4 more convex corners then concave, thus we miss 4 * 1/4 (= + 1) perimeter space after performing P / 2.
         * Thus, Result = A + (P / 2) + 1
         */
        return A + (P / 2) + 1;
    }

    // From https://en.wikipedia.org/wiki/Polygon#Simple_polygons
    private static long CalculateAreaOfSimplePolygon(IList<Coordinate> points) {
        var sum = 0L;
        var n = points.Count;
        for (int i = 0; i < n; i++) {
            var currentPoint = points[i];
            var nextPoint = points[(i + 1) % n];
            sum += (long)currentPoint.X * nextPoint.Y - (long)nextPoint.X * currentPoint.Y;
        }

        return sum / 2;
    }

    private static int Distance(Coordinate a, Coordinate b) {
        return a.X == b.X
            ? Math.Abs(b.Y - a.Y)
            : Math.Abs(b.X - a.X);
    }

    private static int Perimeter(IList<Coordinate> points) {
        var result = 0;
        for (int i = 0; i < points.Count; i++)
            result += Distance(points[i], points[(i + 1) % points.Count]);
        return result;
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
}
