using System.Globalization;
using System.Text;

public static class CsvExporter
{
    public static void Save(string path, IEnumerable<ScenarioResult> results)
    {
        var sb = new StringBuilder();
        sb.AppendLine("scenario,iteration,mean_travel_time,max_travel_time,total_travel_time,converged");

        foreach (ScenarioResult result in results)
        {
            foreach (IterationStats s in result.History)
            {
                bool converged = result.Converged && s.Iteration == result.Iterations;

                sb.AppendLine(string.Join(",",
                    Escape(result.Name),
                    s.Iteration.ToString(CultureInfo.InvariantCulture),
                    s.MeanTravelTime.ToString(CultureInfo.InvariantCulture),
                    s.MaxTravelTime.ToString(CultureInfo.InvariantCulture),
                    s.TotalTravelTime.ToString(CultureInfo.InvariantCulture),
                    converged ? "true" : "false"));
            }
        }

        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, sb.ToString());
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }

    public static void SaveDemandSweep(string path, IEnumerable<(int Demand, ScenarioResult Baseline, ScenarioResult Modified)> rows, double gapTolerance)
    {
        var sb = new StringBuilder();
        sb.AppendLine("demand,gap_tolerance,baseline_mean,modified_mean,change,baseline_converged,modified_converged,baseline_iterations,modified_iterations");

        foreach (var (demand, baseline, modified) in rows)
        {
            sb.AppendLine(string.Join(",",
                demand.ToString(CultureInfo.InvariantCulture),
                gapTolerance.ToString(CultureInfo.InvariantCulture),
                baseline.MeanTravelTime.ToString(CultureInfo.InvariantCulture),
                modified.MeanTravelTime.ToString(CultureInfo.InvariantCulture),
                (modified.MeanTravelTime - baseline.MeanTravelTime).ToString(CultureInfo.InvariantCulture),
                baseline.Converged ? "true" : "false",
                modified.Converged ? "true" : "false",
                baseline.Iterations.ToString(CultureInfo.InvariantCulture),
                modified.Iterations.ToString(CultureInfo.InvariantCulture)));
        }

        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, sb.ToString());
    }
}