public class AssignmentResult
{
    public bool Converged { get; }
    public int Iterations { get; }
    public int ChangedInLastIteration { get; }

    public AssignmentResult(bool converged, int iterations, int changedInLastIteration)
    {
        Converged = converged;
        Iterations = iterations;
        ChangedInLastIteration = changedInLastIteration;
    }
}