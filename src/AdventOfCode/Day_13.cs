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

    public override ValueTask<string> Solve_1() => new($"{Solve(0)}");

    public override ValueTask<string> Solve_2() => new($"{Solve(1)}");

    private int Solve(int smudges) {
        var result = 0;
        foreach (var pattern in _patterns) {
            int[] rows = pattern.Select(FromBinaryToInt).ToArray();
            var rowResult = FindSymmetryIndex(rows, smudges);
            if (rowResult > 0) {
                result += 100 * rowResult;
                continue;
            }

            int[] cols = Transpose(pattern).Select(FromBinaryToInt).ToArray();
            result += FindSymmetryIndex(cols, smudges);
        }

        return result;
    }

    private static int FindSymmetryIndex(int[] lines, int smudges) {
        int i = 0;
        for (; i < lines.Length - 1; i++) {
            var isSymmetrical = true;
            var smudgesLeft = smudges;
            for (
                int left = i, right = i + 1;
                left >= 0 && right < lines.Length;
                left--, right++
            ) {
                smudgesLeft -= CountDifferentBits(lines[left], lines[right]);
                if (smudgesLeft < 0) {
                    isSymmetrical = false;
                    break;
                }
            }

            if (isSymmetrical && smudgesLeft == 0)
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

    private static int CountDifferentBits(int a, int b) {
        int result = 0;

        int c = a ^ b;
        while (c > 0) {
            if ((c & 1) == 1)
                result++;
            c >>= 1;
        }

        return result;
    }
}
