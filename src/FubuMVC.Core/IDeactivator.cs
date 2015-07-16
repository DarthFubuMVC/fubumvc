using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core
{
    /// <summary>
    /// Marker interface for activators that require some shutdown.
    /// This is mainly used in the Bottles.Topshelf scenarios.
    /// </summary>
    public interface IDeactivator
    {
        void Deactivate(IActivationLog log);
    }
}