namespace FubuMVC.Core.Diagnostics
{
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