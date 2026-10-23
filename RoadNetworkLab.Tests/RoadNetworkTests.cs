using Xunit;

public class RoadNetworkTests
{
    [Fact]
    public void AddRoad_DuplicateId_Throws()
    {
        var net = new RoadNetwork();
        net.AddRoad(1, 0, 1, 10, 0);

        Assert.Throws<ArgumentException>(() => net.AddRoad(1, 1, 2, 5, 0));
    }

    [Fact]
    public void AddRoad_NegativeBaseTime_Throws()
    {
        var net = new RoadNetwork();

        Assert.Throws<ArgumentException>(() => net.AddRoad(1, 0, 1, -1, 0));
    }

    [Fact]
    public void AddRoad_InfiniteCongestion_Throws()
    {
        var net = new RoadNetwork();

        Assert.Throws<ArgumentException>(() => net.AddRoad(1, 0, 1, 1, double.PositiveInfinity));
    }

    [Fact]
    public void RemoveRoad_ExistingRoad_RemovesItFromNetwork()
    {
        var net = new RoadNetwork();
        net.AddRoad(1, 0, 1, 10, 0);
        net.AddRoad(2, 0, 2, 20, 0);

        bool removed = net.RemoveRoad(1);

        Assert.True(removed);
        Assert.Equal(1, net.RoadCount);
        Assert.Single(net.GetOutgoingRoads(0));
        Assert.Equal(3, net.VertexCount);
    }

    [Fact]
    public void RemoveRoad_MissingRoad_ReturnsFalse()
    {
        var net = new RoadNetwork();

        Assert.False(net.RemoveRoad(99));
    }

    [Fact]
    public void GetOutgoingRoads_MissingVertex_Throws()
    {
        var net = new RoadNetwork();

        Assert.Throws<ArgumentException>(() => net.GetOutgoingRoads(5));
    }

    [Fact]
    public void HasRoute_ReachableVertex_ReturnsTrue()
    {
        var net = BraessExample.CreateBaseline();

        Assert.True(net.HasRoute(BraessExample.S, BraessExample.T));
    }

    [Fact]
    public void HasRoute_DirectionMatters_ReturnsFalse()
    {
        var net = BraessExample.CreateBaseline();

        Assert.False(net.HasRoute(BraessExample.T, BraessExample.S));
    }

    [Fact]
    public void HasRoute_SameVertex_ReturnsTrue()
    {
        var net = BraessExample.CreateBaseline();

        Assert.True(net.HasRoute(BraessExample.A, BraessExample.A));
    }

    [Fact]
    public void HasRoute_MissingVertex_Throws()
    {
        var net = BraessExample.CreateBaseline();

        Assert.Throws<ArgumentException>(() => net.HasRoute(0, 99));
    }
}