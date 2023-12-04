namespace AdventOfCode;

public class Day_04 : BaseDay
{
    private readonly string[] _input;

    public Day_04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Part 1");

    public override ValueTask<string> Solve_2() => new($"Part 2");
}
