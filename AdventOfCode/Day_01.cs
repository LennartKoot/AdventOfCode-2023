namespace AdventOfCode;

public class Day_01 : BaseDay
{
    private readonly string[] _input;

    public Day_01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");

    public int Solution_1() {
        const char zero = '0';

        var result = 0;
        foreach (var line in _input) {
            result += 10 * FirstDigit(line) + LastDigit(line);
        }

        int FirstDigit(string line) {
            for (var i = 0; i < line.Length; i++) {
                if (char.IsDigit(line[i])) {
                    return line[i] - zero;
                }
            }

            throw new SolvingException();
        }

        int LastDigit(string line) {
            for (var i = line.Length - 1; i >= 0; i--) {
                if (char.IsDigit(line[i])) {
                    return line[i] - zero;
                }
            }

            throw new SolvingException();
        }

        return result;
    }
}
