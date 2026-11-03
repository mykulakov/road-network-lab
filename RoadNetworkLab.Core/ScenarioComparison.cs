public class ScenarioComparison
{
    public double MeanChange { get; }
    public double MeanChangePercent { get; }
    public int DriversWorse { get; }

    public ScenarioComparison(double meanChange, double meanChangePercent, int driversWorse)
    {
        MeanChange = meanChange;
        MeanChangePercent = meanChangePercent;
        DriversWorse = driversWorse;
    }
}