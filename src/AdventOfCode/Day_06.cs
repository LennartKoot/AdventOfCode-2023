namespace AdventOfCode;

public class Day_06 : BaseDay
{
    private readonly string[] _input;

    public Day_06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new("Solution 2");

    record Game(int Time, int Record);

    public int Solution_1() {
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

    private int CountDistancesPastRecord(Game game) {
        var count = 0;
        for (int i = 1; i < game.Time; i++) {
            var distance = i * (game.Time - i);
            if (distance > game.Record)
                count++;
            else if (count > 0)
                break;
        }
        return count;
    }
}
