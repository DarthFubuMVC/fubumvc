namespace FubuMVC.Core
{
    public class DiagnosticsSettings
    {
        public DiagnosticsSettings()
        {
            MaxRequests = 200;
        }

        public int MaxRequests { get; set; }

        public TraceLevel TraceLevel { get; set; }
    }

    public enum TraceLevel
    {
        Verbose,
        Production,
        None,

    }
}