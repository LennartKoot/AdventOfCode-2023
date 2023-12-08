using System.Windows.Markup;

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

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

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
    
    public long Solution_2() {
        var nodes = _nodes
            .Where(kvp => kvp.Key[^1] == 'A')
            .Select(kvp => kvp.Value);

        /* Cycle Steps - Total Steps - Index into new cycle = 0
         * => Cycles exatly in Total Steps
         * => Find LCM of all Total Steps
         */
        var cycles = nodes.Select(GetPathLengthToTarget).ToArray();
        return LCM(cycles);
    }

    private static long LCM(long[] values) {
        static long LCM_Internal(long a, long b) {
            var div = b / GCD(a, b);
            return a * div;
        }

        return values.Aggregate(LCM_Internal);
    }

    private static long GCD(long a, long b) {
        while (b != 0) {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    private long GetPathLengthToTarget(Node start) {
        var steps = 0;
        var instructionIndex = 0;
        var node = start;
        while(node.Label[^1] != 'Z') {
            var instruction = _instructions[instructionIndex];
            if (instruction == 'L')
                node = _nodes[node.Left];
            else
                node = _nodes[node.Right];

            instructionIndex = (instructionIndex + 1) % _instructions.Length;
            ++steps;
        }

        return steps;
    }

    // Used to analyze paths for part 2. Found out they cycle after finding target.
    private List<(Node, int)> FindPath(Node start, Func<Node, bool> isTarget) {
        var result = new List<(Node, int)>();

        var instructionIndex = 0;
        var node = start;
        while(!isTarget(node)) {
            var instruction = _instructions[instructionIndex];
            if (instruction == 'L')
                node = _nodes[node.Left];
            else
                node = _nodes[node.Right];

            instructionIndex = (instructionIndex + 1) % _instructions.Length;
            result.Add((node, instructionIndex));
        }

        return result;
    }
}
