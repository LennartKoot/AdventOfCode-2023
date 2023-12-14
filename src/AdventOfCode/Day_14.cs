namespace AdventOfCode;

public class Day_14 : BaseDay
{
    private readonly char[][] _input;

    public Day_14()
    {
        _input = File
            .ReadLines(InputFilePath)
            .Select(s => s.ToCharArray())
            .ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solution_1() {
        var result = 0;
        var grid = Transpose(_input);
        foreach (var column in grid) {
            RollBoulders(column);
            for (int i = 0; i < column.Length; i++) {
                if (column[i] == 'O')
                    result += column.Length - i;
            }
        }

        return result;
    }

    private static void RollBoulders(char[] line) {
        var trailingIndex = line.Length - 1;
        for (int i = line.Length - 1; i > 0; i--) {
            var rock = line[i];
            var nextRock = line[i - 1];
            if (rock == 'O' && nextRock == '.') {
                line[i - 1] = 'O';
                line[trailingIndex--] = '.';
            }
            if (rock == '#' || rock == '.') {
                trailingIndex = i - 1;
            }
        }
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
}
