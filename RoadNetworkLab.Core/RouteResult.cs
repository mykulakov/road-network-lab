public class RouteResult
{
    public bool Found { get; }
    public double TotalTime { get; }
    public List<int> Vertices { get; }
    public List<Road> Roads { get; }

    public RouteResult(bool found, double totalTime, List<int> vertices, List<Road> roads)
    {
        Found = found;
        TotalTime = totalTime;
        Vertices = vertices;
        Roads = roads;
    }
}