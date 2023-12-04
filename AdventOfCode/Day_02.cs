using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day_02 : BaseDay
{
    public record CubesSubset(int Red, int Green, int Blue);
    public record Game(int Id, List<CubesSubset> CubesSubsets);

    private readonly List<Game> _input;

    [GeneratedRegex(@"\d+(?=\sred)")]
    private static partial Regex RedRegex();

    [GeneratedRegex(@"\d+(?=\sgreen)")]
    private static partial Regex GreenRegex();

    [GeneratedRegex(@"\d+(?=\sblue)")]
    private static partial Regex BlueRegex();

    public Day_02()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Part 2");

    public int Solution_1() {
        const int maxRed = 12;
        const int maxGreen = 13;
        const int maxBlue = 14;

        var result = 0;
        foreach (var game in _input) {
            if (game.CubesSubsets.All(s =>
                s.Red <= maxRed &&
                s.Green <= maxGreen &&
                s.Blue <= maxBlue
            ))
            result += game.Id;
        }

        return result;
    }

    private List<Game> ParseInput() {

        var result = new List<Game>();
        foreach(var line in File.ReadAllLines(InputFilePath)) {
            var segments = line.Split([';', ':']);
            var gameNumber = int.Parse(segments[0].Where(char.IsDigit).ToArray());

            var subsets = new List<CubesSubset>();
            for (int i = 1; i < segments.Length; i++) {
                var colors = segments[i].Split(',');
                var red = 0; var green = 0; var blue = 0;
                foreach(var color in colors) {
                    var redMatch = RedRegex().Match(color);
                    if (redMatch.Success) {
                       red = int.Parse(redMatch.Value);
                       continue;
                    }
                    var greenMatch = GreenRegex().Match(color);
                    if (greenMatch.Success) {
                        green = int.Parse(greenMatch.Value);
                        continue;
                    }
                    var blueMatch = BlueRegex().Match(color);
                    if (blueMatch.Success) {
                        blue = int.Parse(blueMatch.Value);
                        continue;
                    }
                }
                subsets.Add(new(red, green, blue));
            }

            result.Add(new Game(gameNumber, subsets));
        }

        return result;
    }
}
