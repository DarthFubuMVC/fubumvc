namespace FubuMVC.Core
{
    public class DiagnosticsSettings
    {
        public DiagnosticsSettings()
        {
            MaxRequests = 50;
        }

        public int MaxRequests { get; set; }
    }
}