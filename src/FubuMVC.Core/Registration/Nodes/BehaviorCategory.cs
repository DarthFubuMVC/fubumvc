namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    /// The known categories of <see cref="BehaviorNode"/> instances.
    /// </summary>
    public enum BehaviorCategory
    {
        Call,
        Output,
        Wrapper,
        Authentication,
        Authorization,
        Process,
        Cache,
        Conditional,
        Instrumentation,
        Transaction,
        Compression
    }
}