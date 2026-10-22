public static class BraessExample
{
    public const int S = 0;
    public const int A = 1;
    public const int B = 2;
    public const int T = 3;

    public static RoadNetwork CreateBaseline()
    {
        var network = new RoadNetwork();

        network.AddRoad(1, S, A, 0, 0.01);
        network.AddRoad(2, A, T, 45, 0);
        network.AddRoad(3, S, B, 45, 0);
        network.AddRoad(4, B, T, 0, 0.01);

        return network;
    }

    public static RoadNetwork CreateModified()
    {
        RoadNetwork network = CreateBaseline();

        network.AddRoad(5, A, B, 0, 0);

        return network;
    }

    public static List<Driver> CreateDrivers(int count)
    {
        var drivers = new List<Driver>();

        for (int i = 0; i < count; i++)
        {
            drivers.Add(new Driver(i, S, T));
        }

        return drivers;
    }
}