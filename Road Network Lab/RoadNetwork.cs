public class RoadNetwork
{
    private readonly Dictionary<int, List<Road>> _outgoing = new();
    private readonly Dictionary<int, Road> _roadsById = new();

    public int VertexCount => _outgoing.Count;
    public int RoadCount => _roadsById.Count;

    public IEnumerable<int> Vertices => _outgoing.Keys;
    public IEnumerable<Road> Roads => _roadsById.Values;

    public bool HasVertex(int v) => _outgoing.ContainsKey(v);

    public void AddVertex(int v)
    {
        if (!_outgoing.ContainsKey(v))
        {
            _outgoing[v] = new List<Road>();
        }
    }

    public void AddRoad(int id, int from, int to, double baseTime, double congestion)
    {
        if (_roadsById.ContainsKey(id))
        {
            throw new ArgumentException($"A road with ID {id} already exists");
        }

        AddVertex(from);
        AddVertex(to);

        var road = new Road(id, from, to, baseTime, congestion);
        _outgoing[from].Add(road);
        _roadsById[id] = road;
    }

    public bool RemoveRoad(int id)
    {
        if (_roadsById.TryGetValue(id, out Road road))
        {
            _outgoing[road.From].Remove(road);
            _roadsById.Remove(id);
            return true;
        }
        return false;
    }

    public List<Road> GetOutgoingRoads(int v)
    {
        if (_outgoing.TryGetValue(v, out List<Road> roads))
        {
            return roads;
        }
        throw new ArgumentException($"Vertex {v} does not exist");
    }

    public bool HasRoute(int from, int to)
    {
        throw new NotImplementedException();
    }
}