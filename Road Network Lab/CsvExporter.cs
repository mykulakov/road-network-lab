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
}