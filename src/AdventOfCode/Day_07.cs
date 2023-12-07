namespace AdventOfCode;

public class Day_07 : BaseDay
{
    private readonly string[] _input;

    public Day_07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    public int Solution_1()
    {
        var hands = _input
        .Select(l =>
        {
            var xs = l.Split(' ');
            return new Hand(xs[0], int.Parse(xs[1]));
        })
        .ToList();

        Dictionary<char, int> ranks = new()
        {
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'T', 10 },
            { 'J', 11 },
            { 'Q', 12 },
            { 'K', 13 },
            { 'A', 14 },
        };

        hands.Sort((a, b) => CompareTo(a, b, ranks));
        var result = 0;
        for (int i = 1; i <= hands.Count; i++)
            result += i * hands[i - 1].Bet;

        return result;
    }

    private static int CompareTo(Hand current, Hand other, Dictionary<char, int> ranks)
    {
        var compareType = current.Type.CompareTo(other.Type);
        if (compareType != 0)
            return compareType;

        for (int i = 0; i < current.Cards.Length; i++)
        {
            var currentCardRank = ranks[current.Cards[i]];
            var otherCardRank = ranks[other.Cards[i]];
            var compare = currentCardRank.CompareTo(otherCardRank);
            if (compare != 0)
                return compare;
        }

        return 0;
    }

    public int Solution_2() {
        var hands = _input
        .Select(l =>
        {
            var xs = l.Split(' ');
            return new HandJoker(xs[0], int.Parse(xs[1]));
        })
        .ToList();

        Dictionary<char, int> ranks = new()
        {
            { 'J', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'T', 10 },
            { 'Q', 12 },
            { 'K', 13 },
            { 'A', 14 },
        };

        hands.Sort((a, b) => CompareTo(a, b, ranks));
        var result = 0;
        for (int i = 1; i <= hands.Count; i++)
            result += i * hands[i - 1].Bet;

        return result;
    }
}

public enum HandType : int
{
    HighCard = 0,
    OnePair = 1,
    TwoPair = 2,
    ThreeOfAKind = 3,
    FullHouse = 4,
    FourOfAKind = 5,
    FiveOfAKind = 6,
}

public class Hand
{
    public string Cards => _cards;
    private readonly string _cards;

    public int Bet => _bet;
    private readonly int _bet;

    public HandType Type => _type;
    private readonly HandType _type;

    public Hand(string cards, int bet)
    {
        _cards = cards;
        _bet = bet;
        _type = DetermineType(CountCards());
    }

    private Dictionary<char, int> CountCards() {
        Dictionary<char, int> _cardCount = [];
        foreach (var card in _cards)
        {
            if (_cardCount.TryGetValue(card, out int value))
                _cardCount[card] = ++value;
            else
                _cardCount.Add(card, 1);
        }

        return _cardCount;
    }

    internal virtual HandType DetermineType(Dictionary<char, int> cardCounts)
    {
        var counts = cardCounts.Values.OrderDescending().ToArray();
        return counts switch
        {
        [5] => HandType.FiveOfAKind,
        [4, 1] => HandType.FourOfAKind,
        [3, 2] => HandType.FullHouse,
        [3, ..] => HandType.ThreeOfAKind,
        [2, 2, ..] => HandType.TwoPair,
        [2, ..] => HandType.OnePair,
            _ => HandType.HighCard,
        };
    }
}

public class HandJoker(string cards, int bet) : Hand(cards, bet)
{
    internal override HandType DetermineType(Dictionary<char, int> cardCounts)
    {
        if (!cardCounts.TryGetValue('J', out var jokersCount) || jokersCount == 0)
            return base.DetermineType(cardCounts);

        var counts = cardCounts.Values.OrderDescending().ToArray();
        return counts switch
        {
            [5]                                 => HandType.FiveOfAKind,
            [4, ..]                             => HandType.FiveOfAKind,
            [3, 2]                              => HandType.FiveOfAKind,
            [3, ..]                             => HandType.FourOfAKind,
            [2, 2, ..] when jokersCount == 2    => HandType.FourOfAKind,
            [2, 2, ..]                          => HandType.FullHouse,
            [2, ..]                             => HandType.ThreeOfAKind,
            _                                   => HandType.OnePair,
        };
    }
}
