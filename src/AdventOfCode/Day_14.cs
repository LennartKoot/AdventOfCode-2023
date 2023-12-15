using System.Text;

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

    public override ValueTask<string> Solve_1() => new($"{Solve(1)}");

    public override ValueTask<string> Solve_2() => new($"{Solve(4_000_000_000)}");

    public int Solve(long tilts) {
        var grid = _input;
        var cache = new Dictionary<string, char[][]>();

        var tiltInCycle = 0;
        bool[] reverse = [false, false, true, true]; // Switch between direction for south and east

        string firstCacheHitKey = "";
        long firstCacheHitCounter = 0;
        long secondCacheHitCounter = 0;

        var loopStarted = false;
        var loop = new List<char[][]>();

        for (long i = 0; i < tilts; i++) {
            if (loopStarted)
                loop.Add(grid);

            var key = Key(grid);
            if (cache.TryGetValue(key, out var newGrid)) {
                if (!loopStarted) {
                    firstCacheHitCounter = i;
                    firstCacheHitKey = key;
                    loopStarted = true;
                }
                else if (key == firstCacheHitKey) {
                    secondCacheHitCounter = i;
                    break;
                }
                grid = newGrid;
            }
            else {
                grid = Transpose(grid);
                for (int j = 0; j < grid.Length; j++) {
                    ref var column = ref grid[j];
                    if (reverse[tiltInCycle])
                        RollBouldersReversed(column);
                    else
                        RollBoulders(column);
                }
                tiltInCycle = (tiltInCycle + 1) % 4;
                cache[key] = grid;
            }
        }

        if (loopStarted) {
            var loopPeriod = secondCacheHitCounter - firstCacheHitCounter;
            return CalculateResult(loop[(int)((tilts - firstCacheHitCounter) % loopPeriod) - 1]);
        }
        return tilts % 2 == 0
            ? CalculateResult(grid)
            : CalculateResult(Transpose(grid));

        static int CalculateResult(char[][] grid) {
            var result = 0;
            for (int i = 0; i < grid.Length; i++) {
                var column = grid[i];
                for (int j = 0; j < column.Length; j++) {
                    if (column[j] == 'O')
                        result += grid.Length - i;
                }
            }
            return result;
        }
    }

    private static string Key(char[][] grid) {
        var key = new StringBuilder();
        for (int i = 0; i < grid.Length; i++)
            key.Append(new string(grid[i]));
        return key.ToString();
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

    private static void RollBouldersReversed(char[] line) {
        var trailingIndex = 0;
        for (int i = 0; i < line.Length - 1; i++) {
            var rock = line[i];
            var nextRock = line[i + 1];
            if (rock == 'O' && nextRock == '.') {
                line[i + 1] = 'O';
                line[trailingIndex++] = '.';
            }
            if (rock == '#' || rock == '.') {
                trailingIndex = i + 1;
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
