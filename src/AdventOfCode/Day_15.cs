namespace AdventOfCode;

public class Day_15 : BaseDay
{
    private readonly string _input;

    public Day_15()
    {
        _input = File.ReadAllLines(InputFilePath)[0];
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    private static byte Hash(string input) {
        byte currentValue = 0;
        foreach (var @char in input) {
            currentValue = (byte)((currentValue + @char) * 17);
        }
        return currentValue;
    }

    public int Solution_1() {
        var sum = 0;
        foreach (var sequence in _input.Split(','))
            sum += Hash(sequence);

        return sum;
    }

    class Lens(string Label, int FocalLength)
    {
        public string Label { get; } = Label;
        public int FocalLength { get; set; } = FocalLength;
    }

    public int Solution_2() {
        var boxes = new List<Lens>[256];
        for (int i = 0; i < boxes.Length; i++)
            boxes[i] = [];

        foreach (var sequence in _input.Split(',')) {
            var indexOfOperation = sequence.IndexOfAny(['=','-']);

            var label = sequence[..indexOfOperation];
            var box = Hash(label);
            switch (sequence[indexOfOperation]) {
                case '-':
                    var removeIndex = boxes[box].FindIndex(lens => lens.Label == label);
                    if (removeIndex != -1)
                        boxes[box].RemoveAt(removeIndex);
                    break;
                case '=':
                    var focalLength = sequence[^1] - '0';
                    var lens = boxes[box].FirstOrDefault(lens => lens.Label == label);
                    if (lens != null) {
                        lens.FocalLength = focalLength;
                    }
                    else
                        boxes[box].Add(new(label, focalLength));
                    break;
            }
        }

        var focussingPower = 0;
        for (int i = 0; i < boxes.Length; i++) {
            var boxValue = i + 1;
            var box = boxes[i];
            for (int j = 0; j < box.Count; j++) {
                focussingPower += boxValue * (j + 1) * box[j].FocalLength;
            }
        }
        return focussingPower;
    }
}
