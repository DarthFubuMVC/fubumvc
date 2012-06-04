namespace FubuMVC.Core.Diagnostics
{
    [MoveToDiagnostics]
    public interface IDebugCallHandler
    {
        void Handle();
    }
}