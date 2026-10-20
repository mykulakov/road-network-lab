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
}