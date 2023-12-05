namespace AdventOfCode;

public class Day_05 : BaseDay
{
    record Range(long DestinationStart, long SourceStart, long Length);
    record SeedRange(long Start, long Length);

    private long[] _seeds;
    private readonly List<Range>[] _maps = [
        new(), new(), new(), new(), new(), new(), new()
    ];

    public Day_05()
    {
        ParseInput();
    }

    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");
    public override ValueTask<string> Solve_2() => new($"{Solution_2_Optimized()}");

    public long Solution_1() {
        return _seeds.Select(GetLocation).Min();
    }

    public long Solution_2_Original() {
        var seedRanges = new List<SeedRange>();
        for (int i = 0; i < _seeds.Length; i += 2)
            seedRanges.Add(new(_seeds[i], _seeds[i + 1]));

        long location = 0;
        while (true) {
            var seed = GetSeed(location);
            var range = seedRanges.Find(r => r.Start <= seed && r.Start + r.Length > seed);
            if (range != null) {
                return location;
            }
            location++;
        }
    }

    public long Solution_2_Optimized() {
        for (int i = 0; i < _maps.Length; i++) {
            _maps[i].Sort((a, b) => a.SourceStart.CompareTo(b.SourceStart));
        }
        var seedRanges = new List<SeedRange>();
        for (int i = 0; i < _seeds.Length; i += 2)
            seedRanges.Add(new(_seeds[i], _seeds[i + 1]));

        var minLocation = long.MaxValue;

        foreach (var seedRange in seedRanges) {
            var seedRangeMin = seedRange.Start;
            var seedRangeMax = seedRangeMin + seedRange.Length;
            var ranges = OverlapRanges(new(seedRangeMin, seedRangeMin, seedRangeMax - seedRangeMin), _maps[0]);
            for (int i = 1; i < _maps.Length; i++) {
                var newRanges = new List<Range>();
                foreach (var range in ranges) {
                    newRanges.AddRange(OverlapRanges(range, _maps[i]));
                }
                ranges = newRanges;
            }
            var localMinLocation = ranges.MinBy(r => r.SourceStart).SourceStart;
            if (localMinLocation < minLocation)
                minLocation = localMinLocation;
        }

        return minLocation;
    }

    private List<Range> OverlapRanges(Range range, List<Range> sortedSubRanges) {
        if (sortedSubRanges.Count == 0) {
            return [range];
        }

        var rangeMin = range.SourceStart; // Inclusive
        var rangeMax = rangeMin + range.Length; //Exclusive

        var newRanges = new List<Range>();

        var endLastRange = rangeMin;
        for (int i = 0; i < sortedSubRanges.Count; i++) {
            var subRangeMax = sortedSubRanges[i].SourceStart + sortedSubRanges[i].Length;
            if (subRangeMax <= rangeMin || sortedSubRanges[i].SourceStart >= rangeMax)
                continue;

            if (endLastRange < sortedSubRanges[i].SourceStart) {
                newRanges.Add(new(endLastRange, endLastRange, sortedSubRanges[0].SourceStart - endLastRange));
                endLastRange = sortedSubRanges[i].SourceStart;
            }

            var endOfRange = Math.Min(subRangeMax, rangeMax);
            var indexInSubRange = endLastRange - sortedSubRanges[i].SourceStart;
            var startDestination = sortedSubRanges[i].DestinationStart + indexInSubRange;
            newRanges.Add(new(startDestination, startDestination, endOfRange - endLastRange));
            endLastRange = endOfRange;
        }

        if (endLastRange < rangeMax)
            newRanges.Add(new(endLastRange, endLastRange, rangeMax - endLastRange));

        return newRanges;
    }

    private long GetLocation(long source) {
        for (int i = 0; i < 7; i++) {
            var range = _maps[i].Find(r => r.SourceStart <= source && r.SourceStart + r.Length > source);
            if (range != null) {
                var indexInRange = source - range.SourceStart;
                source = range.DestinationStart + indexInRange;
            }
        }
        return source;
    }

    private long GetSeed(long location) {
        var dest = location;
        for (int i = 6; i >= 0; i--) {
            var range = _maps[i].Find(r => r.DestinationStart <= dest && r.DestinationStart + r.Length > dest);
            if (range != null) {
                var indexInRange = dest - range.DestinationStart;
                dest = range.SourceStart + indexInRange;
            }
        }
        return dest;
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
