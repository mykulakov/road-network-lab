var a = ExperimentRunner.RunScenario("Baseline", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(4000), 100);
var b = ExperimentRunner.RunScenario("Modified", BraessExample.CreateModified(), BraessExample.CreateDrivers(4000), 100);

foreach (var r in new[] { a, b })
{
    Console.WriteLine($"{r.Name}: mean={r.MeanTravelTime}, total={r.TotalTravelTime}, converged={r.Converged}, iterations={r.Iterations}");
}

var cmp = ExperimentRunner.Compare(a, b);
Console.WriteLine($"change={cmp.MeanChange}, percent={cmp.MeanChangePercent:F2}, worse={cmp.DriversWorse}");

foreach (var s in a.History)
    Console.WriteLine($"iter {s.Iteration}: mean={s.MeanTravelTime}, max={s.MaxTravelTime}, total={s.TotalTravelTime}");

string csvPath = Path.Combine(FindProjectDirectory(), "results", "results.csv");
CsvExporter.Save(csvPath, new[] { a, b });
Console.WriteLine($"Saved to: {csvPath}");

static string FindProjectDirectory()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);

    while (dir != null && dir.GetFiles("*.csproj").Length == 0)
    {
        dir = dir.Parent;
    }

    return dir?.FullName ?? Directory.GetCurrentDirectory();
}