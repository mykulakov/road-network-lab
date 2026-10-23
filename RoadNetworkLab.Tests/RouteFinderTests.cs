using Xunit;

public class RouteFinderTests
{
    [Fact]
    public void FindShortestPath_BraessNetwork_ReturnsTimeOf45()
    {
        var net = BraessExample.CreateBaseline();

        var result = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.T);

        Assert.True(result.Found);
        Assert.Equal(45, result.TotalTime, 6);
        Assert.Equal(2, result.Roads.Count);
        Assert.Equal(3, result.Vertices.Count);
        Assert.Equal(BraessExample.S, result.Vertices.First());
        Assert.Equal(BraessExample.T, result.Vertices.Last());
    }

    [Fact]
    public void FindShortestPath_SameStartAndEnd_ReturnsZeroLengthPath()
    {
        var net = BraessExample.CreateBaseline();

        var result = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.S);

        Assert.True(result.Found);
        Assert.Equal(0, result.TotalTime, 6);
        Assert.Equal(new List<int> { BraessExample.S }, result.Vertices);
        Assert.Empty(result.Roads);
    }

    [Fact]
    public void FindShortestPath_UnreachableVertex_ReturnsNotFound()
    {
        var net = BraessExample.CreateBaseline();

        var result = RouteFinder.FindShortestPath(net, BraessExample.T, BraessExample.S);

        Assert.False(result.Found);
        Assert.True(double.IsPositiveInfinity(result.TotalTime));
        Assert.Empty(result.Vertices);
        Assert.Empty(result.Roads);
    }

    [Fact]
    public void FindShortestPath_MissingVertex_Throws()
    {
        var net = BraessExample.CreateBaseline();

        Assert.Throws<ArgumentException>(() => RouteFinder.FindShortestPath(net, 0, 99));
        Assert.Throws<ArgumentException>(() => RouteFinder.FindShortestPath(net, 99, 0));
    }

    [Fact]
    public void FindShortestPath_EmptyNetwork_Throws()
    {
        var net = new RoadNetwork();

        Assert.Throws<ArgumentException>(() => RouteFinder.FindShortestPath(net, 0, 1));
    }

    [Fact]
    public void FindShortestPath_EqualCostPaths_ReturnsOneOfThem()
    {
        // Both S->A->T and S->B->T cost 45 when there is no flow.
        var net = BraessExample.CreateBaseline();

        var result = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.T);

        Assert.True(result.Found);
        Assert.Equal(45, result.TotalTime, 6);
        Assert.Contains(result.Vertices[1], new[] { BraessExample.A, BraessExample.B });
    }

    [Fact]
    public void FindShortestPath_ShorterDetourBeatsDirectRoad()
    {
        // Direct 0->1 costs 10, detour 0->2->1 costs 1 + 2 = 3.
        var net = new RoadNetwork();
        net.AddRoad(1, 0, 1, 10, 0);
        net.AddRoad(2, 0, 2, 1, 0);
        net.AddRoad(3, 2, 1, 2, 0);

        var result = RouteFinder.FindShortestPath(net, 0, 1);

        Assert.Equal(3, result.TotalTime, 6);
        Assert.Equal(new List<int> { 0, 2, 1 }, result.Vertices);
        Assert.Equal(new List<int> { 2, 3 }, result.Roads.Select(r => r.Id).ToList());
    }

    [Fact]
    public void FindShortestPath_StaleQueueEntries_DoNotBreakResult()
    {
        // Vertex 1 is queued twice (cost 10, then cost 3); the stale entry must be skipped.
        var net = new RoadNetwork();
        net.AddRoad(1, 0, 1, 10, 0);
        net.AddRoad(2, 0, 2, 1, 0);
        net.AddRoad(3, 2, 1, 2, 0);
        net.AddRoad(4, 1, 3, 20, 0);

        var result = RouteFinder.FindShortestPath(net, 0, 3);

        Assert.Equal(23, result.TotalTime, 6);
        Assert.Equal(new List<int> { 0, 2, 1, 3 }, result.Vertices);
    }

    [Fact]
    public void FindShortestPath_FlowOnRoad_ChangesChosenRoute()
    {
        var net = BraessExample.CreateBaseline();
        foreach (var road in net.Roads.Where(r => r.Id == 1))
        {
            road.Flow = 4000; // S->A now costs 40, so S->A->T costs 85
        }

        var result = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.T);

        Assert.Equal(45, result.TotalTime, 6);
        Assert.Equal(new List<int> { BraessExample.S, BraessExample.B, BraessExample.T }, result.Vertices);
    }
}