using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Runtime
{
    [MoveToDiagnostics]
    public interface IDebugCallHandler
    {
        void Handle();
    }
}