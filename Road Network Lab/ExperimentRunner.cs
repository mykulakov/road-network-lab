public static class ExperimentRunner
{
    public static ScenarioResult RunScenario(string name, RoadNetwork network, List<Driver> drivers, int maxIterations)
    {
        if(drivers.Count == 0)
        {
            throw new ArgumentException($"Scenario '{name}': the list of drivers is empty.");
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
                throw new ArgumentException($"No route from {driver.Start} to {driver.End}");
            }
        }

        AssignmentResult res = TrafficAssignment.RunIterations(network, drivers, maxIterations);

        List<double> driverTimes = drivers.Select(d => d.TravelTime).ToList();

        double maxTravelTime = driverTimes.Max();
        double meanTravelTime = driverTimes.Average();
        double totalTravelTime = driverTimes.Sum();

        return new ScenarioResult(name, network.VertexCount, network.RoadCount, drivers.Count, meanTravelTime, maxTravelTime, totalTravelTime, res.Converged, res.Iterations, driverTimes, res.History);
    }

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
}