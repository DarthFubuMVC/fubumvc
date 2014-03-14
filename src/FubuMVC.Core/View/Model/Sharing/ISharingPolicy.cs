using Bottles.Diagnostics;
using FubuCore;

namespace FubuMVC.Core.View.Model.Sharing
{
    [MarkedForTermination]
    public interface ISharingPolicy
    {
        void Apply(IPackageLog log, ISharingRegistration registration);
    }
}