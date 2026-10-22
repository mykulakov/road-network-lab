public static class TrafficAssignment
{
    public static void AssignRoutes(RoadNetwork network, List<Driver> drivers)
    {
        foreach (Driver driver in drivers)
        {
            driver.Route = RouteFinder.FindShortestPath(network, driver.Start, driver.End);
        }
    }

    public static void ApplyFlows(RoadNetwork network, List<Driver> drivers)
    {
        TrafficModel.ResetFlows(network);

        foreach (Driver driver in drivers){
            if(driver.Route != null){
                TrafficModel.AddFlow(driver.Route, 1);
            }
        }
    }

    public static void UpdateTravelTimes(List<Driver> drivers)
    {
        foreach (Driver driver in drivers)
        {
            if(driver.Route != null)
            {
                driver.TravelTime = TrafficModel.GetRouteTime(driver.Route);
            }
        }
    }

    public static void RunOnePass(RoadNetwork network, List<Driver> drivers)
    {
        AssignRoutes(network, drivers);
        ApplyFlows(network, drivers);
        UpdateTravelTimes(drivers);
    }

    public static bool AreSameRoute(RouteResult? route1, RouteResult? route2)
    {
        if(route1 == null || route2 == null)
        {
            return false;
        }

        return route1.Roads.Select(r => r.Id).SequenceEqual(route2.Roads.Select(r => r.Id));
    }

    public static bool IsRouteRevisedOnK(int driverIndex, int k)
    {
        return driverIndex % k == 0;
    }

    public static double MaxGap(RoadNetwork network, List<Driver> drivers)
    {
        double maxGap = 0;
        var shortestTimes = new Dictionary<(int, int), double>();

        foreach (Driver driver in drivers)
        {
            if (driver.Route == null || !driver.Route.Found)
            {
                continue;
            }

            var key = (driver.Start, driver.End);

            if (!shortestTimes.TryGetValue(key, out double shortest))
            {
                shortest = RouteFinder.FindShortestPath(network, driver.Start, driver.End).TotalTime;
                shortestTimes[key] = shortest;
            }

            double gap = driver.TravelTime - shortest;

            if (gap > maxGap)
            {
                maxGap = gap;
            }
        }

        return maxGap;
    }

    private const double GapTolerance = 1.0;

    public static AssignmentResult RunIterations(RoadNetwork network, List<Driver> drivers, int maxIterations)
    {
        var history = new List<IterationStats>();

        TrafficModel.ResetFlows(network);

        foreach (Driver driver in drivers)
        {
            driver.Route = null;
            driver.TravelTime = double.PositiveInfinity;
        }

        int changed = 0;

        for (int k = 1; k <= maxIterations; k++)
        {
            changed = 0;

            for (int i = 0; i < drivers.Count; i++)
            {
                if (!IsRouteRevisedOnK(i, k))
                {
                    continue;
                }

                Driver driver = drivers[i];
                RouteResult? oldRoute = driver.Route;
                RouteResult newRoute = RouteFinder.FindShortestPath(network, driver.Start, driver.End);

                if (!AreSameRoute(oldRoute, newRoute))
                {
                    changed++;
                }

                driver.Route = newRoute;
            }

            ApplyFlows(network, drivers);
            UpdateTravelTimes(drivers);

            double mean = (drivers.Select(d => d.TravelTime)).Average();
            double max = (drivers.Select(d => d.TravelTime)).Max();
            double sum = (drivers.Select(d => d.TravelTime)).Sum();

            history.Add(new IterationStats(k, mean, max, sum));

            if (MaxGap(network, drivers) <= GapTolerance)
            {
                return new AssignmentResult(true, k, changed, history);
            }
        }

        return new AssignmentResult(false, maxIterations, changed, history);
    }
}