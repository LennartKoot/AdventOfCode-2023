namespace AdventOfCode;

public class Day_11 : BaseDay
{
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public int Solution_1() {
        var galaxies = new List<(int, int)>();
        var lines = File.ReadLines(InputFilePath).ToArray();
        var y = 0;
        foreach (var line in lines) {
            var x = 0;
            foreach (var @char in line) {
                if (@char == '#')
                    galaxies.Add((x, y));
                ++x;
            }
            ++y;
        }

        var max_x = lines[0].Length;
        var max_y = lines.Length;
        var emptyRows = new List<int>();
        var emptyColumns = new List<int>();
        for (int i = 0; i < max_x; i++)
            if (!galaxies.Any(g => g.Item1 == i))
                emptyColumns.Add(i);
        for (int i = 0; i < max_y; i++)
            if (!galaxies.Any(g => g.Item2 == i))
                emptyRows.Add(i);

        var result = 0;
        for (int i = 0; i < galaxies.Count; i++)
            for (int j = i + 1; j < galaxies.Count; j++) {
                var (galaxyX, galaxyY) = galaxies[i];
                var (pairX, pairY) = galaxies[j];
                galaxyX += emptyColumns.Count(c => c < galaxyX);
                galaxyY += emptyRows.Count(r => r < galaxyY);
                pairX += emptyColumns.Count(c => c < pairX);
                pairY += emptyRows.Count(r => r < pairY);

                result += Math.Abs(galaxyX - pairX) + Math.Abs(galaxyY - pairY);
            }

        return result;
    }
}
