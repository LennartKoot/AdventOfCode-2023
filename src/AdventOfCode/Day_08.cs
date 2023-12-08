namespace AdventOfCode;

public class Day_08 : BaseDay
{
    private readonly string[] _input;

    public Day_08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution 1");

    public override ValueTask<string> Solve_2() => new($"Solution 2");
}
