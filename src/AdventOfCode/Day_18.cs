namespace AdventOfCode;

public class Day_18 : BaseDay
{
    private readonly string[] _input;

    public Day_18()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution 1");

    public override ValueTask<string> Solve_2() => new($"Solution 2");
}
