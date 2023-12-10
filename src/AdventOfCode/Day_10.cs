namespace AdventOfCode;

public class Day_10 : BaseDay
{
    private readonly string[] _input;

    public Day_10()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution 1");

    public override ValueTask<string> Solve_2() => new($"Solution 2");
}
