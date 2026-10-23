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
        if (baseTime < 0 || !double.IsFinite(baseTime))
        {
            throw new ArgumentException($"Base time must be a finite number >= 0 (a road with id {id})");
        }

        if (congestion < 0 || !double.IsFinite(congestion))
        {
            throw new ArgumentException($"Congestion must be a finite number >= 0 (a road with id {id})");
        }

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
        if (!HasVertex(from)){
            throw new ArgumentException($"Vertex {from} does not exist");
        }

        if (!HasVertex(to)){
            throw new ArgumentException($"Vertex {to} does not exist");
        }

        var visited = new HashSet<int>();
        Queue<int> q = new Queue<int>();

        visited.Add(from);
        q.Enqueue(from);

        while(q.Count > 0)
        {
            int v = q.Dequeue();

            if (v == to)
            {
                return true;
            }

            foreach (Road road in GetOutgoingRoads(v))
            {
                int next = road.To;

                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    q.Enqueue(next);
                }
            }
        }

        return false;
    }

    public RoadNetwork Clone()
    {
        var copy = new RoadNetwork();

        foreach (int v in _outgoing.Keys)
        {
            copy.AddVertex(v);
        }

        foreach (Road road in _roadsById.Values)
        {
            copy.AddRoad(road.Id, road.From, road.To, road.BaseTime, road.Congestion);
        }

        return copy;
    }
}