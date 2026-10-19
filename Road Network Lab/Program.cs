Console.OutputEncoding = System.Text.Encoding.UTF8;

var braess = new RoadNetwork();

// S=0, A=1, B=2, T=3
braess.AddRoad(1, 0, 1, 0, 0.01);   // S -> A
braess.AddRoad(2, 1, 3, 45, 0);     // A -> T
braess.AddRoad(3, 0, 2, 45, 0);     // S -> B
braess.AddRoad(4, 2, 3, 0, 0.01);   // B -> T

Console.WriteLine("--- Main case: 0 -> 3 ---");
PrintResult(RouteFinder.FindShortestPath(braess, 0, 3));

Console.WriteLine("--- Same start and end: 0 -> 0 ---");
PrintResult(RouteFinder.FindShortestPath(braess, 0, 0));

Console.WriteLine("--- No route: 3 -> 0 ---");
PrintResult(RouteFinder.FindShortestPath(braess, 3, 0));

Console.WriteLine("--- Missing vertex: 0 -> 99 ---");
try
{
    RouteFinder.FindShortestPath(braess, 0, 99);
    Console.WriteLine("ERROR: exception was not thrown");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Expected exception: {ex.Message}");
}

Console.WriteLine("--- Duplicates in queue: 0 -> 1 ---");
var dup = new RoadNetwork();
dup.AddRoad(1, 0, 1, 10, 0);   // 0 -> 1, cost 10
dup.AddRoad(2, 0, 2, 1, 0);    // 0 -> 2, cost 1
dup.AddRoad(3, 2, 1, 2, 0);    // 2 -> 1, cost 2
PrintResult(RouteFinder.FindShortestPath(dup, 0, 1));

Console.WriteLine("--- Flow test: 0 -> 3 with Flow = 4000 on S->A ---");
braess.GetOutgoingRoads(0)[0].Flow = 4000;   // road 1: S -> A
PrintResult(RouteFinder.FindShortestPath(braess, 0, 3));

TrafficModel.ResetFlows(braess);
Console.WriteLine($"Flow on road 1 after reset: {braess.GetOutgoingRoads(0)[0].Flow}");

Console.WriteLine("--- AddFlow test ---");
var route = RouteFinder.FindShortestPath(braess, 0, 3);   // Flow is 0 everywhere, so path is 0 -> 1 -> 3
TrafficModel.AddFlow(route, 1000);
foreach (var road in braess.Roads)
{
    Console.WriteLine($"Road {road.Id}: flow = {road.Flow}");
}

Console.WriteLine($"Route time under flow 1000: {TrafficModel.GetRouteTime(route)}");

static void PrintResult(RouteResult result)
{
    Console.WriteLine($"Found: {result.Found}");
    Console.WriteLine($"Total time: {result.TotalTime}");
    Console.WriteLine($"Vertices: {string.Join(" -> ", result.Vertices)}");
    Console.WriteLine($"Roads: {string.Join(", ", result.Roads.Select(r => r.Id))}");
}