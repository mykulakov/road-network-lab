using System.Globalization;

public static class ConsoleReport
{
    public static void PrintScenario(ScenarioResult result)
    {
        Console.WriteLine($"--- {result.Name} ---");
        Console.WriteLine($"Vertices: {result.VertexCount}, roads: {result.RoadCount}, drivers: {result.DriverCount}");
        Console.WriteLine($"Mean travel time: {Format(result.MeanTravelTime)}");
        Console.WriteLine($"Max travel time: {Format(result.MaxTravelTime)}");
        Console.WriteLine($"Total travel time: {Format(result.TotalTravelTime)}");
        Console.WriteLine($"Converged: {(result.Converged ? "yes" : "no")}, iterations: {result.Iterations}");

        if (!result.Converged)
        {
            Console.WriteLine("WARNING: the assignment did not converge. This is NOT an equilibrium, treat the numbers above as unstable.");
        }

        Console.WriteLine();
    }

    public static void PrintComparison(ScenarioResult baseline, ScenarioResult modified, ScenarioComparison comparison)
    {
        Console.WriteLine($"--- Comparison: {baseline.Name} -> {modified.Name} ---");

        string percent = double.IsNaN(comparison.MeanChangePercent)
            ? "n/a"
            : comparison.MeanChangePercent.ToString("+0.00;-0.00;0.00", CultureInfo.InvariantCulture) + "%";

        Console.WriteLine($"Mean travel time change: {comparison.MeanChange.ToString("+0.##;-0.##;0", CultureInfo.InvariantCulture)} ({percent})");
        Console.WriteLine($"Drivers with longer travel time: {comparison.DriversWorse} of {baseline.DriverCount}");

        string verdict = comparison.MeanChange > 1e-9 ? "WORSE"
                       : comparison.MeanChange < -1e-9 ? "BETTER"
                       : "UNCHANGED";
        Console.WriteLine($"Result: average travel time became {verdict}.");

        if (!baseline.Converged || !modified.Converged)
        {
            Console.WriteLine("WARNING: at least one scenario did not converge, so this comparison is unreliable.");
        }

        Console.WriteLine();
    }

    private static string Format(double value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}