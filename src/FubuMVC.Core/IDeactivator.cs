using Bottles.Diagnostics;

namespace Bottles
{
    /// <summary>
    /// Marker interface for activators that require some shutdown.
    /// This is mainly used in the Bottles.Topshelf scenarios.
    /// </summary>
    public interface IDeactivator
    {
        void Deactivate(IPackageLog log);
    }
}