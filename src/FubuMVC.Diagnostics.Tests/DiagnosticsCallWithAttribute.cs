namespace FubuMVC.Diagnostics.Tests
{
    public class DiagnosticsCallWithAttribute
    {
        [FubuDiagnosticsUrl("~/my-extension")]
        public void Execute()
        {
        }
    }
}