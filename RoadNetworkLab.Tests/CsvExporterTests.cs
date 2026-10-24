using Xunit;

public class CsvExporterTests
{
    [Fact]
    public void Save_ScenarioNameWithComma_IsQuoted()
    {
        var result = ExperimentRunner.RunScenario(
            "A,B", BraessExample.CreateBaseline(), BraessExample.CreateDrivers(100), 500);
        string path = Path.Combine(Path.GetTempPath(), $"road-lab-{Guid.NewGuid()}.csv");

        try
        {
            CsvExporter.Save(path, new[] { result });
            var lines = File.ReadAllLines(path);

            Assert.Equal("scenario,iteration,mean_travel_time,max_travel_time,total_travel_time,converged", lines[0]);
            Assert.StartsWith("\"A,B\",1,", lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }
}