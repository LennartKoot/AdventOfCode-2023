namespace AdventOfCode;

public class Day_08 : BaseDay
{
    private readonly char[] _instructions;
    private readonly Dictionary<string,Node> _nodes = [];

    public Day_08()
    {
        var input = File.ReadAllLines(InputFilePath);
        _instructions = input[0].ToCharArray();

        for (int i = 2; i < input.Length; i++) {
            var line = input[i];
            var label = line.Substring(0, 3);
            var left = line.Substring(7, 3);
            var right = line.Substring(12, 3);
            _nodes.Add(label, new(label, left, right));
        }
    }

    record Node(string Label, string Left, string Right);
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solution_1() {
        var step = 0;

        var node = _nodes["AAA"];
        while(node.Label != "ZZZ") {
            var instruction = _instructions[step % _instructions.Length];
            if (instruction == 'L')
                node = _nodes[node.Left];
            else
                node = _nodes[node.Right];
            ++step;
        }

        return step;
    }
}
