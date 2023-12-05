using AoCHelper;

namespace AdventOfCode.Test;

public static class SolutionTests
{
    [TestCase(typeof(Day_01), "54597", "54504")]
    [TestCase(typeof(Day_02), "2551", "62811")]
    [TestCase(typeof(Day_03), "528819", "80403602")]
    [TestCase(typeof(Day_04), "22897", "5095824")]
    [TestCase(typeof(Day_05), "486613012", "56931769")]
    public static async Task Test(Type type, string sol1, string sol2)
    {
        if (Activator.CreateInstance(type) is BaseProblem instance)
        {
            await Assert.ThatAsync(async () => await instance.Solve_1(), Is.EqualTo(sol1));
            await Assert.ThatAsync(async () => await instance.Solve_2(), Is.EqualTo(sol2));
        }
        else
        {
            Assert.Fail($"{type} is not a BaseDay");
        }
    }
}