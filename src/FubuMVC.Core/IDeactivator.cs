using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core
{
    /// <summary>
    /// Marker interface for activators that require some shutdown.
    /// </summary>
    public interface IDeactivator
    {
        void Deactivate(IActivationLog log);
    }
}