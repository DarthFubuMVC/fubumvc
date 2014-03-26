namespace FubuMVC.Diagnostics.Runtime
{
    public class TracedStep<T>
    {
        public TracedStep(T log, double time)
        {
            Log = log;
            RequestTimeInMilliseconds = time;
        }

        public T Log { get; private set; }
        public double RequestTimeInMilliseconds { get; private set; }
    }
}