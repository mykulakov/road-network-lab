public class IterationStats
{
    public int Iteration { get; }
    public double MeanTravelTime { get; }
    public double MaxTravelTime { get; }
    public double TotalTravelTime { get; }

    public IterationStats(int iteration, double meanTravelTime, double maxTravelTime, double totalTravelTime)
    {
        Iteration = iteration;
        MeanTravelTime = meanTravelTime;
        MaxTravelTime = maxTravelTime;
        TotalTravelTime = totalTravelTime;
    }
}