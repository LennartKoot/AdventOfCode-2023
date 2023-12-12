using Stack = System.Collections.Immutable.ImmutableStack<int>;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;
using System.Collections.Immutable;

namespace AdventOfCode;

public class Day_12 : BaseDay
{
    private readonly string[] _input;

    public Day_12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    public long Solution_1() {
        long result = 0;
        foreach (var line in _input) {
            var parts = line.Split(' ');
            var springs = parts[0];
            var numbers = parts[1].Split(',').Select(int.Parse);
            result += GetPossibleArrangements(springs, ImmutableStack.CreateRange(numbers.Reverse()), new Cache());
        }
        return result;
    }

     public long Solution_2() {
        long result = 0;
        foreach (var line in _input) {
            var parts = line.Split(' ');
            var springs = Unfold(parts[0], '?', 5);
            var numbers = Unfold(parts[1], ',', 5).Split(',').Select(int.Parse);
            result += GetPossibleArrangements(springs, ImmutableStack.CreateRange(numbers.Reverse()), new Cache());
        }
        return result;
    }

    private static string Unfold(string input, char seperator, int repeat) {
        return string.Join(seperator, Enumerable.Repeat(input, repeat));
    }

    private static long GetPossibleArrangements(string springs, Stack configuration, Cache cache) {
        if (!cache.TryGetValue((springs, configuration), out var result)) {
            var calculatedResult = CalculatePossibleArrangements(springs, configuration, cache);
            cache[(springs, configuration)] = calculatedResult;
            return calculatedResult;
        }

        return result;
    }

    private static long CalculatePossibleArrangements(string springs, Stack numbers, Cache cache) {
        return springs.FirstOrDefault() switch {
            '.' => GetPossibleArrangements(springs[1..], numbers, cache), // Continue recurse with operational spring removed.
            '#' => HandleBrokenSpring(springs, numbers, cache),
            '?' => GetPossibleArrangements("." + springs[1..], numbers, cache) + GetPossibleArrangements("#" + springs[1..], numbers, cache), // Recurse with trying both an operational and broken spring.
            _   => !numbers.IsEmpty ? 0 : 1, // If numbers left at end of sequence, this is not a valid pattern.
        };
    }

    private static long HandleBrokenSpring(string springs, Stack numbers, Cache cache) {
        if (numbers.IsEmpty)
            return 0; // Broken spring while configuration says no broken springs left.

        numbers = numbers.Pop(out var n);

        var potentialBrokenSprings = 0;
        for (;potentialBrokenSprings < springs.Length; potentialBrokenSprings++) {
            var spring = springs[potentialBrokenSprings];
            if (spring == '.')
                break;
        }

        if (potentialBrokenSprings < n)
            return 0; // Can't reach the required amount of broken strings set in the configuration.
        if (n == springs.Length)
            return numbers.IsEmpty ? 1 : 0; // End of springs, if we still need broken strings this is an invalid arrangement.
        if (springs[n] == '#')
            return 0; // Current required sequence is directly followed by another broken spring, invalid.

        return GetPossibleArrangements(springs[(n+1)..], numbers, cache); // Continue recurse with next sequence of springs.
    }
}
