namespace AdventOfCode;

public class Day_20 : BaseDay
{
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

    public long Solution_1() {
        var system = SignalSystem.Create(InputFilePath);
        for (int i = 0; i < 1000; i++)
            system.PressButton();

        return system.LowCount * system.HighCount;
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
        private readonly IEnumerable<string> _outputs;

        public Broadcaster(IEnumerable<string> outputs) {
            _outputs = outputs;
        }

        public IEnumerable<string> ProcessPulse(Pulse pulse, string from)
        {
            return _outputs;
        }

        public Pulse GetOutputPulse()
        {
            return Pulse.Low;
        }
    }

    class FlipFlop : IModule
    {
        private readonly IEnumerable<string> _outputs;
        private bool _isOn = false;

        public FlipFlop(IEnumerable<string> outputs) {
            _outputs = outputs;
        }

        public IEnumerable<string> ProcessPulse(Pulse pulse, string from)
        {
            if (pulse == Pulse.High)
                return Enumerable.Empty<string>();
            _isOn = !_isOn;
            return _outputs;
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
