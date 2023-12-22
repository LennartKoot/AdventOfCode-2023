using System.Globalization;

namespace AdventOfCode;

public class Day_20 : BaseDay
{
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    public long Solution_1() {
        var system = SignalSystem.Create(InputFilePath);
        for (int i = 0; i < 1000; i++)
            system.PressButton();

        return system.LowCount * system.HighCount;
    }

    public long Solution_2() {
        /* Put input into GraphViz to visualise flow.
         * rx receives input ONLY from gf, which is a conjunction
         * gf receives input from kf, kr, qk, zs, which are ALL conjunctions
         * All these inputs have themselves only one input: gm, bf, qr, cx, all conjunctions
         * These 9 inputs are the only conjunctions, everything else is flip flops
         * Broadcaster sends a pulse to one flip-flop in each of the four groups connected to conjunctions gm, bf, qr and cx
         * After these flip-flops is a whole chain of flip-flops up to their groups conjunction
         * Some of these flip-flops are input into the conjunction
         * If a flip flop receives a low pulse, it switches on and off. A high pulse is ignored.
         * Representing on and off as 1 and 0, the chain will look like this in a few button presses:
         * 1. 1000.... => ...0001
         * 2. 0100.... => ...0010
         * 3. 1100.... => ...0011
         * 4. 0010.... => ...0100
         * => The flip flops are a counter
         * => If all flip flops connected to the conjunction are 1, the conjunction sends a low signal
         * => The next conjunction receives the low signal and sends a high signal to rx
         * => Flip flop groups cycle, find LCM where each group of flip flops connected to conjunction are on for first time.
         * => Create binary number where we only set a 1 if the flip flop is connected to the conjunction
         * => This will be the first 'number' in the counter where all required flip flops are on in the group.
         */

        var system = SignalSystem.Create(InputFilePath);
        var broadcaster = system.Modules["broadcaster"];
        var startFlipFlops = (broadcaster as Broadcaster).Outputs;
        var binaryNumbers = new List<string>();
        foreach (var startFlipFlop in startFlipFlops) {
            binaryNumbers.Add(buildBinaryStringForFlipFlop(system.Modules[startFlipFlop] as FlipFlop));
        }

        return LCM(binaryNumbers.Select(s => long.Parse(s, NumberStyles.BinaryNumber)).ToArray());
        string buildBinaryStringForFlipFlop(FlipFlop flipflop) {
            var outputs = flipflop.Outputs.ToArray();
            if (outputs.Length == 1) {
                var module = system.Modules[outputs[0]];
                if (module is Conjunction) // End of flip flop chain
                    return "1";
                else return buildBinaryStringForFlipFlop(module as FlipFlop) + "0";
            }
            var nextFlipFlop = system.Modules[outputs[0]];
            if (nextFlipFlop is not FlipFlop)
                nextFlipFlop = system.Modules[outputs[1]];

            return buildBinaryStringForFlipFlop(nextFlipFlop as FlipFlop) + "1";
        }
    }

    private static long LCM(long[] values) {
        static long LCM_Internal(long a, long b) {
            var div = b / GCD(a, b);
            return a * div;
        }

        return values.Aggregate(LCM_Internal);
    }

    private static long GCD(long a, long b) {
        while (b != 0) {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    enum Pulse {
        Low = 0,
        High = 1,
    }

    interface IModule {
        IEnumerable<string> ProcessPulse(Pulse pulse, string from);
        Pulse GetOutputPulse();
    }

    class Broadcaster : IModule
    {
        public IEnumerable<string> Outputs;

        public Broadcaster(IEnumerable<string> outputs) {
            Outputs = outputs;
        }

        public IEnumerable<string> ProcessPulse(Pulse pulse, string from)
        {
            return Outputs;
        }

        public Pulse GetOutputPulse()
        {
            return Pulse.Low;
        }
    }

    class FlipFlop : IModule
    {
        public IEnumerable<string> Outputs;
        private bool _isOn = false;

        public FlipFlop(IEnumerable<string> outputs) {
            Outputs = outputs;
        }

        public IEnumerable<string> ProcessPulse(Pulse pulse, string from)
        {
            if (pulse == Pulse.High)
                return Enumerable.Empty<string>();
            _isOn = !_isOn;
            return Outputs;
        }

        public Pulse GetOutputPulse() {
            return _isOn ? Pulse.High : Pulse.Low;
        }
    }

    class Conjunction : IModule
    {
        private readonly IEnumerable<string> _outputs;
        private readonly Dictionary<string, Pulse> _inputs;

        public Conjunction(IEnumerable<string> outputs, IEnumerable<string> inputs)
        {
            _outputs = outputs;
            _inputs = [];
            foreach (var input in inputs)
                _inputs.Add(input, Pulse.Low);
        }

        public IEnumerable<string> ProcessPulse(Pulse pulse, string from)
        {
            _inputs[from] = pulse;
            return _outputs;
        }

        public Pulse GetOutputPulse() {
            return _inputs.All(kvp => kvp.Value == Pulse.High) ? Pulse.Low : Pulse.High;
        }
    }

    class SignalSystem {
        record Signal(string From, string To);

        public IReadOnlyDictionary<string, IModule> Modules => _modules;

        private readonly Dictionary<string, IModule> _modules;
        private readonly Queue<Signal> _queue = new();

        public long LowCount => _lowCount;
        private long _lowCount = 0;
        public long HighCount => _highCount;
        private long _highCount = 0;

        SignalSystem(Dictionary<string, IModule> modules) {
            _modules = modules;
        }

        public void PressButton() {
            _queue.Enqueue(new("broadcaster", "broadcaster"));
            ProcessTillEmpty();
        }

        public IModule GetModule(string name) {
            return _modules[name];
        }

        private void ProcessTillEmpty() {
            while(_queue.Count > 0) {
                var signal = _queue.Dequeue();
                Pulse pulse = _modules[signal.From].GetOutputPulse();
                if (pulse == Pulse.Low)
                    ++_lowCount;
                else
                    ++_highCount;

                if (!_modules.TryGetValue(signal.To, out var module))
                    continue;
                var outputs = module.ProcessPulse(pulse, signal.From);
                foreach (var output in outputs)
                    _queue.Enqueue(new Signal(signal.To, output));
            }
        }

        enum ModuleType {
            FlipFlop = 0,
            Conjunction = 1,
            Broadcaster = 2,
        }

        public static SignalSystem Create(string inputFilePath) {
            Dictionary<string, (ModuleType, string[])> modules = []; 
            Dictionary<string, List<string>> inputs = [];

            foreach (var line in File.ReadAllLines(inputFilePath)) {
                var splitted = line.Split("->", StringSplitOptions.TrimEntries);
                var module = splitted[0];
                var outputs = splitted[1].Split(',', StringSplitOptions.TrimEntries);

                var moduleType = module[0] switch {
                    '%' => ModuleType.FlipFlop,
                    '&' => ModuleType.Conjunction,
                    'b' => ModuleType.Broadcaster,
                    _ => throw new SolvingException(),
                };

                var moduleName = moduleType switch {
                    ModuleType.Broadcaster => "broadcaster",
                    _ => module[1..],
                };

                foreach (var output in outputs) {
                    if (!inputs.TryGetValue(output, out var moduleInputs))
                        inputs.Add(output, [moduleName]);
                    else
                        moduleInputs.Add(moduleName);
                }

                modules.Add(moduleName, (moduleType, outputs));
            }

            Dictionary<string, IModule> fullModules = [];

            foreach (var module in modules) {
                var (name, (type, outputs)) = module;
                switch (type) {
                    case ModuleType.Broadcaster:
                        fullModules.Add(name, new Broadcaster(outputs));
                        break;
                    case ModuleType.FlipFlop:
                        fullModules.Add(name, new FlipFlop(outputs));
                        break;
                    case ModuleType.Conjunction:
                        fullModules.Add(name, new Conjunction(outputs, inputs[name]));
                        break;
                }
            }

            return new SignalSystem(fullModules);
        }
    }
}
