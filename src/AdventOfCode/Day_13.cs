using System.Globalization;

namespace AdventOfCode;

public class Day_13 : BaseDay
{
    private readonly List<char[][]> _patterns;

    public Day_13()
    {
        _patterns = [];
        var lines = File.ReadLines(InputFilePath).GetEnumerator();
        while (lines.MoveNext()) {
            List<char[]> patternLines = [];
            while (lines.Current != string.Empty) {
                patternLines.Add(
                    lines.Current.Select(c => c == '.' ? '0' : '1').ToArray()
                );
                if (!lines.MoveNext())
                    break;
            }
            _patterns.Add([.. patternLines]);
        }
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solution_1() {
        var result = 0;
        foreach (var pattern in _patterns) {
            int[] rows = pattern.Select(FromBinaryToInt).ToArray();
            int[] cols = Transpose(pattern).Select(FromBinaryToInt).ToArray();

            result += 100 * FindSymmetryIndex(rows);
            result += FindSymmetryIndex(cols);
        }

        return result;
    }

    private static int FindSymmetryIndex(int[] lines) {
        int i = 0;
        for (; i < lines.Length - 1; i++) {
            var isSymmetrical = true;
            for (
                int left = i, right = i + 1;
                left >= 0 && right < lines.Length;
                left--, right++
            ) {
                if (lines[left] != lines[right]) {
                    isSymmetrical = false;
                    break;
                }
            }

            if (isSymmetrical)
                return i + 1;
        }

        return 0;
    }

    private static char[][] Transpose(char[][] input) {
        var x_length = input[0].Length;
        var y_length = input.Length;

        var result = new char[x_length][];

        for (int x = 0; x < x_length; x++) {
            result[x] = new char[y_length];
            for (int y = 0; y < y_length; y++) {
                result[x][y] = input[y][x];
            }
        }

        return result;
    }

    private static int FromBinaryToInt(char[] binary) {
        return int.Parse(binary, NumberStyles.BinaryNumber);
    }
}
