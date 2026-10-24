using Xunit;

public class ExperimentRunnerTests
{
    [Fact]
    public void RunScenario_EmptyDrivers_Throws()
    {
        var net = BraessExample.CreateBaseline();

        Assert.Throws<ArgumentException>(() =>
            ExperimentRunner.RunScenario("Test", net, new List<Driver>(), 100));
    }

    [Fact]
    public void RunScenario_ZeroMaxIterations_Throws()
    {
        var net = BraessExample.CreateBaseline();
        var drivers = BraessExample.CreateDrivers(10);

        Assert.Throws<ArgumentException>(() =>
            ExperimentRunner.RunScenario("Test", net, drivers, 0));
    }

    [Fact]
    public void RunScenario_UnreachablePair_Throws()
    {
        var net = BraessExample.CreateBaseline();
        var drivers = new List<Driver> { new Driver(0, BraessExample.T, BraessExample.S) };

        Assert.Throws<ArgumentException>(() =>
            ExperimentRunner.RunScenario("Test", net, drivers, 100));
    }

    [Fact]
    public void RunScenario_Baseline4000_ComputesExpectedStatistics()
    {
        var result = ExperimentRunner.RunScenario(
            "Baseline", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(4000), 500);

        Assert.True(result.Converged);
        Assert.Equal(65, result.MeanTravelTime, 6);
        Assert.Equal(65, result.MaxTravelTime, 6);
        Assert.Equal(260000, result.TotalTravelTime, 6);
        Assert.Equal(4, result.VertexCount);
        Assert.Equal(4, result.RoadCount);
        Assert.Equal(4000, result.DriverCount);
        Assert.Equal(4000, result.DriverTimes.Count);
    }

    [Fact]
    public void RunScenario_IterationLimitOfOne_DoesNotConverge()
    {
        var result = ExperimentRunner.RunScenario(
            "Limited", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(4000), 1);

        Assert.False(result.Converged);
        Assert.Equal(1, result.Iterations);
    }

    [Fact]
    public void Compare_DifferentDriverCounts_Throws()
    {
        var small = ExperimentRunner.RunScenario(
            "Small", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(100), 500);
        var big = ExperimentRunner.RunScenario(
            "Big", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(200), 500);

        Assert.Throws<ArgumentException>(() => ExperimentRunner.Compare(small, big));
    }

    [Fact]
    public void Compare_BraessParadox_ModifiedIsWorseForEveryone()
    {
        var baseline = ExperimentRunner.RunScenario(
            "Baseline", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(4000), 500);
        var modified = ExperimentRunner.RunScenario(
            "Modified", BraessExample.CreateModified(), BraessExample.CreateDrivers(4000), 500);

        var cmp = ExperimentRunner.Compare(baseline, modified);

        Assert.Equal(15, cmp.MeanChange, 6);
        Assert.Equal(23.0769, cmp.MeanChangePercent, 3);
        Assert.Equal(4000, cmp.DriversWorse);
    }

    [Fact]
    public void Compare_ScenarioWithItself_ShowsNoChange()
    {
        var result = ExperimentRunner.RunScenario(
            "Baseline", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(4000), 500);

        var cmp = ExperimentRunner.Compare(result, result);

        Assert.Equal(0, cmp.MeanChange, 6);
        Assert.Equal(0, cmp.DriversWorse);
    }
}