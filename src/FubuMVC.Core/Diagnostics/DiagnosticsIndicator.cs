using FubuCore;

namespace FubuMVC.Core.Diagnostics
{
    [MarkedForTermination("just silly.  Put this on BehaviorGraph")]
    public class DiagnosticsIndicator
    {
        public DiagnosticsIndicator()
        {
            IsDiagnosticsEnabled = false;
        }

        public DiagnosticsIndicator SetEnabled()
        {
            IsDiagnosticsEnabled = true;
            return this;
        }
        
        public bool IsDiagnosticsEnabled
        {
            get;
            private set;
        }
    }
}