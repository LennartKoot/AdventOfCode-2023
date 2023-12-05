﻿namespace AdventOfCode;

public class Day_05 : BaseDay
{
    record Range(long DestinationStart, long SourceStart, long Length);

    private long[] _seeds;
    private readonly List<Range>[] _maps = [
        new(), new(), new(), new(), new(), new(), new()
    ];

    public Day_05()
    {
        ParseInput();
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");
    public override ValueTask<string> Solve_2() => new($"Part 2");

    public long Solution_1() {
        return _seeds.Select(GetLocation).Min();
    }

    private long GetLocation(long seed) {
        var source = seed;
        for (int i = 0; i < 7; i++) {
            var range = _maps[i].Find(r => r.SourceStart <= source && r.SourceStart + r.Length >= source);
            if (range != null) {
                var indexInRange = source - range.SourceStart;
                source = range.DestinationStart + indexInRange;
            }
        }
        return source;
    }

    private void ParseInput() {
        var lines = File.ReadAllLines(InputFilePath);
        _seeds = lines[0]
            .Split(' ')
            .Skip(1)
            .Select(long.Parse)
            .ToArray();
        
        var mapI = 0;
        int i = 3;
        while(i < lines.Length) {
            var line = lines[i];
            if (string.IsNullOrEmpty(line)) {
                mapI++;
                i += 2; // Skip to first range of next map
                continue;
            }

            var range = line
                .Split(' ')
                .Select(long.Parse)
                .ToArray();
            _maps[mapI].Add(new(range[0], range[1], range[2]));
            i++;
        }
    }
}