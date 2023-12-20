namespace AdventOfCode;

public class Day_19 : BaseDay
{
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"{Solution_2()}");

    public long Solution_1() {
        var (workflows, parts) = Parse();
        var map = workflows.ToDictionary(w => w.Name);

        var result = 0L;
        foreach (var part in parts)
            if (GetTargetForPart(map, part, "in") == "A")
                result += part.X + part.M + part.A + part.S;

        return result;
    }

    private static string GetTargetForPart(Dictionary<string, Workflow> workflows, Part part, string workflowName) {
        var workflow = workflows[workflowName];
        var i = 0;
        string target;
        while (!workflow.Rules[i].Evaluate(part, out target))
            ++i;
        if (target == "A" || target == "R")
            return target;
        
        return GetTargetForPart(workflows, part, target);
    }

    public long Solution_2() {
        var (workflows, _) = Parse();
        var map = workflows.ToDictionary(w => w.Name);
        return FindMaxCombinations(map, new(1, 4000), new(1, 4000), new(1, 4000), new(1, 4000), "in");
    }

    record Range(int Min, int Max) {
        public int Size => Max - Min + 1;

        public (Range, Range) Split(int pivot) {
            return (new(Min, pivot), new(pivot + 1, Max));
        }
    }

    private static long FindMaxCombinations(Dictionary<string, Workflow> workflows, Range x, Range m, Range a, Range s, string workflowName) {
        if (workflowName == "A")
            return (long)x.Size * m.Size * a.Size * s.Size;
        if (workflowName == "R")
            return 0;
        var workflow = workflows[workflowName];
        var result = 0L;
        for (int i = 0; i < workflow.Rules.Count; i++) {
            var rule = workflow.Rules[i];
            if (rule is FallbackRule) {
                result += FindMaxCombinations(workflows, x, m, a, s, rule.Target);
                continue;
            }
            var castedRule = rule as Rule;
            Range ruleRange;
            Range newRange;
            switch (castedRule.Category) {
                case 'x':
                    if (!TrySplitOnRule(x, castedRule, out ruleRange, out newRange))
                        continue;
                    else {
                        result += FindMaxCombinations(workflows, ruleRange, m, a, s, rule.Target);
                        x = newRange;
                    }
                    break;
                case 'm':
                    if (!TrySplitOnRule(m, castedRule, out ruleRange, out newRange))
                        continue;
                    else {
                        result += FindMaxCombinations(workflows, x, ruleRange, a, s, rule.Target);
                        m = newRange;
                    }
                    break;
                case 'a':
                    if (!TrySplitOnRule(a, castedRule, out ruleRange, out newRange))
                        continue;
                    else {
                        result += FindMaxCombinations(workflows, x, m, ruleRange, s, rule.Target);
                        a = newRange;
                    }
                    break;
                case 's':
                    if (!TrySplitOnRule(s, castedRule, out ruleRange, out newRange))
                        continue;
                    else {
                        result += FindMaxCombinations(workflows, x, m, a, ruleRange, rule.Target);
                        s = newRange;
                    }
                    break;
            }
        }

        return result;

        static bool TrySplitOnRule(Range range, Rule rule, out Range ruleRange, out Range newRange) {
            ruleRange = null;
            newRange = null;
            if (rule.Operation == Operation.LessThan && rule.RHS <= range.Min)
                return false;
            if (rule.Operation == Operation.GreaterThan && rule.RHS >= range.Max)
                return false;

            var pivot = rule.Operation == Operation.LessThan ? rule.RHS - 1 : rule.RHS;
            var (left, right) = range.Split(pivot);
            ruleRange = rule.Operation == Operation.LessThan ? left : right;
            newRange = rule.Operation == Operation.LessThan ? right : left;
            return true;
        }
    }

    private (ICollection<Workflow>, ICollection<Part>) Parse() {
        var iterLines = File.ReadLines(InputFilePath).GetEnumerator();

        var workflows = new List<Workflow>();
        while(iterLines.MoveNext()) {
            var line = iterLines.Current;
            if (string.IsNullOrEmpty(line))
                break;
            
            var splitted = line.Split('{');
            var workflowName = splitted[0];
            var rules = splitted[1][..^1].Split(',');
            var workflowRules = new List<IRule>();
            for (int i = 0; i < rules.Length - 1; i++) {
                var ruleSplitted = rules[i].Split(':');
                var rule = ruleSplitted[0];
                var category = rule[0];
                var operation = rule[1] == '<' ? Operation.LessThan : Operation.GreaterThan;
                var lhs = int.Parse(rule[2..]);
                var target = ruleSplitted[1];
                workflowRules.Add(new Rule(category, operation, lhs, target));
            }
            workflowRules.Add(new FallbackRule(rules[^1]));
            workflows.Add(new(workflowName, workflowRules));
        }

        var parts = new List<Part>();
        while(iterLines.MoveNext()) {
            var line = iterLines.Current[1..^1];
            var categories = line.Split(',');
            var x = int.Parse(categories[0][2..]);
            var m = int.Parse(categories[1][2..]);
            var a = int.Parse(categories[2][2..]);
            var s = int.Parse(categories[3][2..]);
            parts.Add(new(x, m, a, s));
        }

        return (workflows, parts);
    }

    record Part(int X, int M, int A, int S);

    enum Operation {
        LessThan = 0,
        GreaterThan = 1,
    }

    interface IRule {
        string Target { get; }
        bool Evaluate(Part part, out string target);
    }

    record Rule(char Category, Operation Operation, int RHS, string Target) : IRule
    {
        bool PerformOperation(int LHS) {
            return Operation switch {
                Operation.LessThan      => LHS < RHS,
                Operation.GreaterThan   => LHS > RHS,
                _ => throw new SolvingException(),
            };
        }

        public bool Evaluate(Part part, out string target) {
            var result = Category switch {
                'x' => PerformOperation(part.X),
                'm' => PerformOperation(part.M),
                'a' => PerformOperation(part.A),
                's' => PerformOperation(part.S),
                _ => throw new SolvingException(),
            };
            target = result ? Target : string.Empty;
            return result;
        }
    }

    record FallbackRule(string Target) : IRule
    {
        public bool Evaluate(Part part, out string target)
        {
            target = Target;
            return true;
        }
    }

    record Workflow(string Name, IList<IRule> Rules);
}
