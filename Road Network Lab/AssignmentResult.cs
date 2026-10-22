public class AssignmentResult
{
    public bool Converged { get; }
    public int Iterations { get; }
    public int ChangedInLastIteration { get; }
    public List<IterationStats> History { get; }

    public AssignmentResult(bool converged, int iterations, int changedInLastIteration, List<IterationStats> history)
    {
        Converged = converged;
        Iterations = iterations;
        ChangedInLastIteration = changedInLastIteration;
        History = history;
    }
}