public static class RouteFinder
{
    public static RouteResult FindShortestPath(RoadNetwork network, int start, int end)
    {
        if (!network.HasVertex(start))
        {
            throw new ArgumentException($"Vertex {start} does not exist");
        }

        if (!network.HasVertex(end))
        {
            throw new ArgumentException($"Vertex {end} does not exist");
        }

        var prevRoad = new Dictionary<int, Road>();

        var pq = new PriorityQueue<int, double>();
        pq.Enqueue(start, 0);

        var dist = new Dictionary<int, double>();

        foreach (int v in network.Vertices)
        {
            dist[v] = double.PositiveInfinity;
        }

        dist[start] = 0;

        while (pq.TryDequeue(out int u, out double d))
        {
            if (d > dist[u])
            {
                continue;
            }

            if (u == end)
            {
                break;
            }

            foreach (var road in network.GetOutgoingRoads(u))
            {
                double roadTime = TrafficModel.GetTravelTime(road);
                double newDist = dist[u] + roadTime;

                if (newDist < dist[road.To])
                {
                    dist[road.To] = newDist;
                    prevRoad[road.To] = road;
                    pq.Enqueue(road.To, newDist);
                }
            }
        }

        if (double.IsPositiveInfinity(dist[end]))
        {
            return new RouteResult(false, double.PositiveInfinity, new List<int>(), new List<Road>());
        }

        var roads = new List<Road>();
        int current = end;

        while (current != start)
        {
            Road road = prevRoad[current];
            roads.Add(road);
            current = road.From;
        }

        roads.Reverse();

        var vertices = new List<int> { start };

        foreach (var r in roads)
        {
            vertices.Add(r.To);
        }

        return new RouteResult(true, dist[end], vertices, roads);
    }
}