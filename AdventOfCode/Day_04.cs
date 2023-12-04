namespace AdventOfCode;

public class Day_04 : BaseDay
{
    private readonly string[] _input;

    public Day_04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    public int Solution_1() {
        var result = 0;
        foreach (var card in _input) {
            var splitted = card.Split([':', '|']);
            var winning = splitted[1].Split(' ').Where(s => !string.IsNullOrEmpty(s));
            var owned = splitted[2].Split(' ').Where(s => !string.IsNullOrEmpty(s));

            var winningNumbers = owned.Count(winning.Contains);
            if (winningNumbers > 0)
                result += 1 << (winningNumbers - 1);
        }

        return result;
    }

    public int Solution_2() {
        Dictionary<int, int> cards = [];
        for (int i = 0; i < _input.Length; i++) {
            if (!cards.TryAdd(i, 1))
                cards[i]++;

            var card = _input[i];

            var splitted = card.Split([':', '|']);
            var winning = splitted[1].Split(' ').Where(s => !string.IsNullOrEmpty(s));
            var owned = splitted[2].Split(' ').Where(s => !string.IsNullOrEmpty(s));

            var winningNumbers = owned.Count(winning.Contains);

            var currentCardsOwned = cards[i];

            for (int j = 1; j <= winningNumbers; j++) {
                if (cards.TryGetValue(i + j, out int value))
                    cards[i + j] = value + currentCardsOwned;
                else
                    cards.Add(i + j, currentCardsOwned);
            }
        }

        return cards.Sum(kvp => kvp.Value);
    }
}
