using Xunit;

public class TrafficAssignmentTests
{
    [Fact]
    public void RunIterations_NegativeTolerance_Throws()
    {
        var net = BraessExample.CreateBaseline();
        var drivers = BraessExample.CreateDrivers(10);

        Assert.Throws<ArgumentException>(() =>
            TrafficAssignment.RunIterations(net, drivers, 10, -1));
    }

    [Fact]
    public void RunIterations_Baseline_ConvergesInTwoIterations()
    {
        var net = BraessExample.CreateBaseline();
        var drivers = BraessExample.CreateDrivers(4000);

        var result = TrafficAssignment.RunIterations(net, drivers, 500);

        Assert.True(result.Converged);
        Assert.Equal(2, result.Iterations);
    }

    [Fact]
    public void IsRouteRevisedOnK_RevisesOnlyDriversDivisibleByK()
    {
        Assert.True(TrafficAssignment.IsRouteRevisedOnK(0, 5));
        Assert.True(TrafficAssignment.IsRouteRevisedOnK(6, 3));
        Assert.False(TrafficAssignment.IsRouteRevisedOnK(4, 3));
        Assert.True(TrafficAssignment.IsRouteRevisedOnK(7, 1));
    }

    [Fact]
    public void AreSameRoute_NullRoute_ReturnsFalse()
    {
        var net = BraessExample.CreateBaseline();
        var route = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.T);

        Assert.False(TrafficAssignment.AreSameRoute(null, route));
        Assert.False(TrafficAssignment.AreSameRoute(route, null));
    }

    [Fact]
    public void AreSameRoute_SameAndDifferentRoutes()
    {
        var net = BraessExample.CreateBaseline();
        var first = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.T);
        var again = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.T);

        foreach (var road in net.Roads.Where(r => r.Id == 1))
        {
            road.Flow = 4000; // forces the route through B
        }
        var other = RouteFinder.FindShortestPath(net, BraessExample.S, BraessExample.T);

        Assert.True(TrafficAssignment.AreSameRoute(first, again));
        Assert.False(TrafficAssignment.AreSameRoute(first, other));
    }

    [Fact]
    public void MaxGap_AfterOneIteration_IsFortyOnBaseline()
    {
        // After one iteration everyone drives S->A->T (time 85),
        // while S->B->T would now cost 45, so the gap is 40.
        var net = BraessExample.CreateBaseline();
        var drivers = BraessExample.CreateDrivers(4000);

        TrafficAssignment.RunIterations(net, drivers, 1);

        Assert.Equal(40, TrafficAssignment.MaxGap(net, drivers), 6);
    }
}