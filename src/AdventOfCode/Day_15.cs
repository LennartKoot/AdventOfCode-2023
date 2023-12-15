namespace AdventOfCode;

public class Day_15 : BaseDay
{
    private readonly string _input;

    public Day_15()
    {
        _input = File.ReadAllLines(InputFilePath)[0];
    }

    public override ValueTask<string> Solve_1() => new($"{Solve()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solve() {
        var sum = 0;
        foreach (var sequence in _input.Split(',')) {
            var currentValue = 0;
            foreach (var @char in sequence) {
                currentValue = (currentValue + @char) * 17 % 256;
            }
            sum += currentValue;
        }

        return sum;
    }
}
