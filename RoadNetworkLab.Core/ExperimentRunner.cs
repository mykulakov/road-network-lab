public static class ExperimentRunner
{
    /// <summary>
    /// Runs the traffic assignment for one scenario and collects statistics.
    /// Note: modifies the passed network (road flows) and drivers (routes, travel times),
    /// so each scenario needs its own network and driver list.
    /// </summary>
    public static ScenarioResult RunScenario(string name, RoadNetwork network, List<Driver> drivers, int maxIterations, double gapTolerance = TrafficAssignment.DefaultGapTolerance)
    {
        if(drivers.Count == 0)
        {
            throw new ArgumentException($"Scenario '{name}': the list of drivers is empty.");
        }

        if (maxIterations < 1)
        {
            throw new ArgumentException($"Scenario '{name}': maxIterations must be at least 1.");
        }

        var checkedPairs = new HashSet<(int, int)>();

        foreach (Driver driver in drivers)
        {
            var pair = (driver.Start, driver.End);

            if (!checkedPairs.Add(pair))
            {
                continue;
            }

            if (!network.HasRoute(driver.Start, driver.End))
            {
                throw new ArgumentException($"Scenario '{name}': no route from {driver.Start} to {driver.End}.");
            }
        }

        AssignmentResult res = TrafficAssignment.RunIterations(network, drivers, maxIterations, gapTolerance);

        List<double> driverTimes = drivers.Select(d => d.TravelTime).ToList();

        double maxTravelTime = driverTimes.Max();
        double meanTravelTime = driverTimes.Average();
        double totalTravelTime = driverTimes.Sum();

        return new ScenarioResult(name, network.VertexCount, network.RoadCount, drivers.Count, meanTravelTime, maxTravelTime, totalTravelTime, res.Converged, res.Iterations, driverTimes, res.History);
    }

    // drivers are matched by index, which is valid because both scenarios use identical demand
    public static ScenarioComparison Compare(ScenarioResult baseline, ScenarioResult modified)
    {
        if (baseline.DriverTimes.Count != modified.DriverTimes.Count)
        {
            throw new ArgumentException("Scenarios have different numbers of drivers, comparison is not meaningful.");
        }

        double meanChange = modified.MeanTravelTime - baseline.MeanTravelTime;

        double meanChangePercent = baseline.MeanTravelTime == 0
            ? double.NaN
            : meanChange / baseline.MeanTravelTime * 100;

        int driversWorse = 0;

        for (int i = 0; i < baseline.DriverTimes.Count; i++)
        {
            double before = baseline.DriverTimes[i];
            double after = modified.DriverTimes[i];

            if (after > before + 1e-9)
            {
                driversWorse++;
            }
        }

        return new ScenarioComparison(meanChange, meanChangePercent, driversWorse);
    }

    public static List<(int Demand, ScenarioResult Baseline, ScenarioResult Modified)> RunDemandSweep(IEnumerable<int> demands, int maxIterations, double gapTolerance = TrafficAssignment.DefaultGapTolerance)
    {
        var rows = new List<(int, ScenarioResult, ScenarioResult)>();

        foreach (int demand in demands)
        {
            var baseline = RunScenario($"Baseline {demand}", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(demand), maxIterations, gapTolerance);
            var modified = RunScenario($"Modified {demand}", BraessExample.CreateModified(), BraessExample.CreateDrivers(demand), maxIterations, gapTolerance);
            rows.Add((demand, baseline, modified));
        }

        return rows;
    }
}