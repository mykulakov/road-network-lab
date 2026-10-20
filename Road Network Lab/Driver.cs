public class Driver
{
    public int Id { get; }
    public int Start { get; }
    public int End { get; }

    public RouteResult? Route { get; set; }
    public double TravelTime { get; set; }

    public Driver(int id, int start, int end){
        Id = id;
        Start = start;
        End = end;
    }
}