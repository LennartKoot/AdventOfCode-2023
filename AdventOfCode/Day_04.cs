namespace AdventOfCode;

public class Day_04 : BaseDay
{
    private readonly string[] _input;

    public Day_04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Part 2");

    public int Solution_1() {
        var result = 0;
        foreach (var card in _input) {
            var splitted = card.Split([':', '|']);
            var winning = splitted[1].Split(' ').Where(s => !string.IsNullOrEmpty(s));
            var owned = splitted[2].Split(' ').Where(s => !string.IsNullOrEmpty(s));

            var winningNumbers = owned.Count(winning.Contains);
            if (winningNumbers > 0)
                result += 1 << (winningNumbers - 1);
        }

        return result;
    }
}
