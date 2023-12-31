using AoCHelper;

namespace AdventOfCode.Test;

public static class SolutionTests
{
    [TestCase(typeof(Day_01), "54597", "54504")]
    [TestCase(typeof(Day_02), "2551", "62811")]
    [TestCase(typeof(Day_03), "528819", "80403602")]
    [TestCase(typeof(Day_04), "22897", "5095824")]
    [TestCase(typeof(Day_05), "486613012", "56931769")]
    [TestCase(typeof(Day_06), "781200", "49240091")]
    [TestCase(typeof(Day_07), "250453939", "248652697")]
    [TestCase(typeof(Day_08), "14429", "10921547990923")]
    [TestCase(typeof(Day_09), "1887980197", "990")]
    [TestCase(typeof(Day_10), "6842", "393")]
    [TestCase(typeof(Day_11), "10313550", "611998089572")]
    [TestCase(typeof(Day_12), "7204", "1672318386674")]
    [TestCase(typeof(Day_13), "33356", "28475")]
    [TestCase(typeof(Day_14), "108641", "84328")]
    [TestCase(typeof(Day_15), "512797", "262454")]
    [TestCase(typeof(Day_16), "7060", "7493")]
    [TestCase(typeof(Day_17), "855", "980")]
    [TestCase(typeof(Day_18), "92758", "62762509300678")]
    [TestCase(typeof(Day_19), "373302", "130262715574114")]
    [TestCase(typeof(Day_20), "739960225", "231897990075517")]
    [TestCase(typeof(Day_21), "3788", "631357596621921")]
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
