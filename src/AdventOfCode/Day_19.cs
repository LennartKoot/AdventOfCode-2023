using Microsoft.VisualBasic;

namespace AdventOfCode;

public class Day_19 : BaseDay
{
    public override ValueTask<string> Solve_1() => new($"{Solution_1()}");

    public override ValueTask<string> Solve_2() => new($"Solution 2");

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
