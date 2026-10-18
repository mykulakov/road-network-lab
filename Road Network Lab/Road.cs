public class Road
{
	public int Id { get; }
    public int From { get; }
    public int To { get; }
    public double BaseTime { get; }
    public double Congestion { get; }
    public double Flow { get; set; }

    public Road(int id, int from, int to, double baseTime, double congestion)
    {
        Id = id;
        From = from;
        To = to;
        BaseTime = baseTime;
        Congestion = congestion;
        Flow = 0;
    }
}
