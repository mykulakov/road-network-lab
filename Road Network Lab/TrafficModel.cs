public static class TrafficModel
{
    public static double GetTravelTime(Road road)
    {
        return road.BaseTime + road.Congestion * road.Flow;
    }

    public static void ResetFlows(RoadNetwork network)
    {
        foreach(Road road in network.Roads)
        {
            road.Flow = 0;
        }
    }

    public static void AddFlow(RouteResult route, double amount)
    {
        foreach (Road road in route.Roads)
        {
            road.Flow += amount;
        }
    }

    public static double GetRouteTime(RouteResult route)
    {
        double total = 0;

        foreach (Road road in route.Roads)
        {
            total += GetTravelTime(road);
        }

        return total;
    }
}