using FubuMVC.Core.Registration;

namespace FubuMVC.Core
{
    [ApplicationLevel]
    public class DiagnosticsSettings
    {
        private TraceLevel? _traceLevel;

        public DiagnosticsSettings()
        {
            MaxRequests = 200;

            if (FubuMode.InDevelopment())
            {
                _traceLevel = TraceLevel.Verbose;
            }

        }

        public int MaxRequests { get; set; }

        public TraceLevel TraceLevel    
        {
            get { return _traceLevel ?? TraceLevel.None; }
            set { _traceLevel = value; }
        }

        public void SetIfNone(TraceLevel level)
        {
            if (!_traceLevel.HasValue)
            {
                _traceLevel = level;
            }
        }
    }

    public enum TraceLevel
    {
        Verbose,
        Production,
        None,

    }
}