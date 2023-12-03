namespace AdventOfCode;

public class Day_02 : BaseDay
{
    private readonly string[] _input;

    public Day_02()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Part 1");

    public override ValueTask<string> Solve_2() => new($"Part 2");
}
