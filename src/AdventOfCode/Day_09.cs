using Spectre.Console;

namespace AdventOfCode;

public class Day_09 : BaseDay
{
    private readonly string[] _input;

    public Day_09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public long Solution_1() {
        long result = 0;
        foreach (var report in _input) {
            var numbers = report.Split(' ').Select(n => (int?)int.Parse(n)).ToArray();
            List<int?[]> sequences = [numbers];

            var currentSequence = 0;
            while (sequences[currentSequence][^1] != 0) {
                sequences.Add(new int?[sequences[currentSequence].Length - 1]);
                currentSequence++;
                DetermineNumberInSequence(sequences, currentSequence);
            }

            for(; currentSequence >= 0; currentSequence--)
                result += (int)sequences[currentSequence][^1];
        }

        return result;
    }

    private static int DetermineNumberInSequence(List<int?[]> sequences, int sequence, int indexFromEnd = 1) {
        var lastValue = sequences[sequence][^indexFromEnd];
        if (lastValue != null)
            return (int)lastValue;

        var left = DetermineNumberInSequence(sequences, sequence - 1, indexFromEnd + 1);
        var right = DetermineNumberInSequence(sequences, sequence -1, indexFromEnd);
        var value = right - left;
        sequences[sequence][^indexFromEnd] = value;
        return value;
    }
}
