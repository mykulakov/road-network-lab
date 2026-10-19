public static class TrafficModel
{
    public static double GetTravelTime(Road road)
    {
        return road.BaseTime + road.Congestion * road.Flow;
    }
}