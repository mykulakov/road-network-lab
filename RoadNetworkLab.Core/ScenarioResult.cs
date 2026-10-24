public class ScenarioResult
{
    public string Name { get; }
    public int VertexCount { get; }
    public int RoadCount { get; }
    public int DriverCount { get; }
    public double MeanTravelTime { get; }
    public double MaxTravelTime { get; }
    public double TotalTravelTime { get; }
    public bool Converged { get; }
    public int Iterations { get; }
    public List<double> DriverTimes { get; }
    public List<IterationStats> History { get; }

    public ScenarioResult(string name, int vertexCount, int roadCount, int driverCount, double meanTravelTime, double maxTravelTime, double totalTravelTime, bool converged, int iterations, List<double> driverTimes, List<IterationStats> history)
    {
        Name = name;
        VertexCount = vertexCount;
        RoadCount = roadCount;
        DriverCount = driverCount;
        MeanTravelTime = meanTravelTime;
        MaxTravelTime = maxTravelTime;
        TotalTravelTime = totalTravelTime;
        Converged = converged;
        Iterations = iterations;
        DriverTimes = driverTimes;
        History = history;
    }
}