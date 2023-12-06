namespace AdventOfCode;

public class Day_06 : BaseDay
{
    private readonly string[] _input;

    public Day_06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2_QuadraticFormula()}");

    record Game(long Time, long Record);

    public long Solution_1() {
        var times = _input[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(int.Parse)
            .ToArray();
        var records = _input[1]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(int.Parse)
            .ToArray();

        var games = new List<Game>();
        for (int i = 0; i < times.Length; i++) {
            games.Add(new(times[i], records[i]));
        }

        return games
            .Select(CountDistancesPastRecord)
            .Aggregate((a, b) => a * b);
    }

    public long Solution_2_Original() {
        var time = ConvertDigitsToNumber(_input[0].Split(':')[1]);
        var record = ConvertDigitsToNumber(_input[1].Split(':')[1]);

        return CountDistancesPastRecord(new(time, record));
    }

    public long Solution_2_QuadraticFormula() {
        var time = ConvertDigitsToNumber(_input[0].Split(':')[1]);
        var record = ConvertDigitsToNumber(_input[1].Split(':')[1]);

        /* => x * (T - x) = R
         * => -(x^2) + T*x = R
         * => x^2 - T*x = -R
         * => x^2 - T*x + R = 0
         * => a = 1, b = -T, c = R
         * where T = time and R = record
         */
        (var x1, var x2) = QuadraticFormula(1, -time, record);
        return (long)Math.Ceiling(x1) - (long)Math.Floor(x2) - 1; // -1 because x1 and x2 are exclusive bounds
    }

    private static (double, double) QuadraticFormula(long a, long b, long c) {
        var sqrt = Math.Sqrt(b * b - 4 * a * c);
        var x1 = (-b + sqrt) / (2 * a);
        var x2 = (-b - sqrt) / (2 * a);

        return (x1, x2);
    }

    private long CountDistancesPastRecord(Game game) {
        long count = 0;
        for (long i = 1; i < game.Time; i++) {
            var distance = i * (game.Time - i);
            if (distance > game.Record)
                count++;
            else if (count > 0)
                break;
        }
        return count;
    }

    private long ConvertDigitsToNumber(string digits) {
        return long.Parse(digits.Replace(" ", string.Empty));
    }
}
