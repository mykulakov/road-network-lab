const double Tolerance = 0.01;

var a = ExperimentRunner.RunScenario("Baseline", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(4000), 500, Tolerance);
var b = ExperimentRunner.RunScenario("Modified", BraessExample.CreateModified(), BraessExample.CreateDrivers(4000), 500, Tolerance);

ConsoleReport.PrintScenario(a);
ConsoleReport.PrintScenario(b);
ConsoleReport.PrintComparison(a, b, ExperimentRunner.Compare(a, b));

string resultsDir = Path.Combine(FindProjectDirectory(), "results");

CsvExporter.Save(Path.Combine(resultsDir, "results.csv"), new[] { a, b });
Console.WriteLine($"Saved: {Path.Combine(resultsDir, "results.csv")}");

var rows = ExperimentRunner.RunDemandSweep(new[] { 500, 1000, 2000, 3000, 4000, 5000 }, 500, Tolerance);

Console.WriteLine();
Console.WriteLine("--- Demand sweep ---");
Console.WriteLine("demand | baseline mean | modified mean | change | converged (base/mod)");
foreach (var (demand, baseline, modified) in rows)
{
    double change = modified.MeanTravelTime - baseline.MeanTravelTime;
    Console.WriteLine($"{demand} | {baseline.MeanTravelTime:0.##} | {modified.MeanTravelTime:0.##} | {change:+0.##;-0.##;0} | {baseline.Converged}/{modified.Converged}");
}

CsvExporter.SaveDemandSweep(Path.Combine(resultsDir, "demand_sweep.csv"), rows, Tolerance);
Console.WriteLine($"Saved: {Path.Combine(resultsDir, "demand_sweep.csv")}");

var limited = ExperimentRunner.RunScenario("Baseline (1 iteration)", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(4000), 1);
Console.WriteLine();
ConsoleReport.PrintScenario(limited);

static string FindProjectDirectory()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);

    while (dir != null && dir.GetFiles("*.csproj").Length == 0)
    {
        dir = dir.Parent;
    }

    return dir?.FullName ?? Directory.GetCurrentDirectory();
}