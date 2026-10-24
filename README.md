# Road Network Lab

A C# console app that simulates drivers choosing routes on a small road network and reproduces the **Braess paradox**: adding a road can make everyone's trip longer.

Graph, Dijkstra's algorithm and the traffic assignment are written from scratch, without graph libraries.

## Result

On the classic Braess network with 4000 drivers, adding a free road A -> B raises the average trip time from 65 to 80 (+23%), and every driver ends up slower.

Whether the extra road helps depends on demand:

| Drivers | Without A -> B | With A -> B | Change |
|--------:|---------------:|------------:|-------:|
| 500     | 47.5           | 10          | -37.5  |
| 2000    | 55             | 40          | -15    |
| 3000    | 60             | 60          | 0      |
| 4000    | 65             | 80          | +15    |
| 5000    | 70             | 90          | +20    |

Below about 3000 drivers the new road helps, above that it hurts. Full data is in [`Road Network Lab/results/demand_sweep.csv`](Road%20Network%20Lab/results/demand_sweep.csv).

## How it works

- The network is a directed graph stored as an adjacency list. The travel time of a road is `a + b * x`, where `a` is the time on an empty road, `x` is the number of drivers currently on it, and `b` is how much each extra driver slows it down. For example, `a = 0, b = 0.01` means 1000 drivers add 10 to the travel time, while `b = 0` is a road that never slows down.
- Every driver wants the shortest route, but routes get slower as they fill up. If everyone switches at once, traffic just swings between routes, so on iteration `k` only about `1/k` of the drivers re-choose their route (method of successive averages).
- The loop stops when no driver could save more than a small tolerance by switching, or when the iteration limit is hit. If the limit is hit first, the result is reported as not converged.

## Run

Requires the [.NET SDK](https://dotnet.microsoft.com/download) (the project targets .NET 10).

```
git clone https://github.com/mykulakov/road-network-lab.git
cd road-network-lab
dotnet run --project "Road Network Lab"
dotnet test
```

The program prints the scenario report and comparison, runs the demand sweep, and writes CSV files to `results/`.

## Limitations

- The equilibrium is approximate and depends on the tolerance. At 5000 drivers the result with tolerance 1 is 88.74, while 0.01 gives the exact 90 (but takes 317 iterations instead of 7). The table above uses 0.01.
- Drivers who re-choose a route are picked by index, so driver 0 re-chooses on every iteration.
- The network and demand are defined in code.
