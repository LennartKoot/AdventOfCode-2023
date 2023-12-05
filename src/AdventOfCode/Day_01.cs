namespace AdventOfCode;

public class Day_01 : BaseDay
{
    private readonly string[] _input;

    public Day_01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

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

    public int Solution_2() {
        const char zero = '0';
        const char nine = '9';

        var result = 0;
        foreach(var line in _input) {
            var digits = new List<int>(8);
            foreach(var window in SlidingWindow(line, 5)) {
                if (TryGetDigitInWindow(window, out var digit)) {
                    digits.Add(digit);
                }
            }

            result += 10 * digits[0] + digits[^1];
        }

        return result;

        static bool TryGetDigitInWindow(IList<char> xs, out int digit) {
            switch (xs)
            {
                case [>= zero and <= nine and var c, ..]:
                    digit = c - zero;
                    return true;
                case ['z', 'e', 'r', 'o', ..]:
                    digit = 0;
                    return true;
                case ['o', 'n', 'e', ..]:
                    digit = 1;
                    return true;
                case ['t', 'w', 'o', ..]:
                    digit = 2;
                    return true;
                case ['t', 'h', 'r', 'e', 'e', ..]:
                    digit = 3;
                    return true;
                case ['f', 'o', 'u', 'r', ..]:
                    digit = 4;
                    return true;
                case ['f', 'i', 'v', 'e', ..]:
                    digit = 5;
                    return true;
                case ['s', 'i', 'x', ..]:
                    digit = 6;
                    return true;
                case ['s', 'e', 'v', 'e', 'n', ..]:
                    digit = 7;
                    return true;
                case ['e', 'i', 'g', 'h', 't', ..]:
                    digit = 8;
                    return true;
                case ['n', 'i', 'n', 'e', ..]:
                    digit = 9;
                    return true;
                default:
                    digit = -1;
                    return false;
            };
        }
    }

    private static IEnumerable<IList<char>> SlidingWindow(string line, int size) {
        var window = new List<char>(size);

        for (var i = 0; i < line.Length; i++) {
            window.Add(line[i]);
            if (window.Count < size) {
                continue;
            }

            var nextWindow = new List<char>(window.Skip(1));
            yield return window;
            window = nextWindow;
        }
        while (window.Count > 0) {
            var nextWindow = new List<char>(window.Skip(1));
            yield return window;
            window = nextWindow;
        }
    }
}
