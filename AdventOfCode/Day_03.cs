namespace AdventOfCode;

public class Day_03 : BaseDay
{
    private readonly string[] _input;

    public Day_03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Part 2");

    public int Solution_1() {
        var result = 0;

        var max_x = _input[0].Length;
        var max_y = _input.Length;

        for (int y = 0; y < max_y; y++) {
            List<char> digits = new(3);
            bool hasAdjacentSymbol = false;

            for (int x = 0; x < max_x; x++) {
                var c = _input[y][x];

                if (char.IsDigit(c)) {
                    digits.Add(c);

                    if (hasAdjacentSymbol)
                        continue;

                    hasAdjacentSymbol = CheckNeighboursForSymbol(_input, x, y);
                }
                else {
                    if (hasAdjacentSymbol)
                        result += int.Parse(digits.ToArray());
                    digits.Clear();
                    hasAdjacentSymbol = false;
                }
            }

            if (hasAdjacentSymbol)
                result += int.Parse(digits.ToArray());
        }

        return result;
    }

    private static bool CheckNeighboursForSymbol(string[] lines, int x, int y) {
        var max_x = lines[0].Length;
        var max_y = lines.Length;

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++) {
                if (dx == 0 && dy == 0)
                    continue;

                int x2 = x + dx;
                int y2 = y + dy;

                if (0 <= x2 && x2 < max_x
                &&  0 <= y2 && y2 < max_y) {
                    var c2 = lines[y2][x2];
                    if (c2 != '.' && !char.IsDigit(c2)) {
                        return true;
                    }
                }
            }

        return false;
    }
}
