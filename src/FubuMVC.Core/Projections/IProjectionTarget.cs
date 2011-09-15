using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Projections
{
    public interface IProjectionTarget
    {
        object ValueFor(Accessor accessor);
        IUrlRegistry Urls { get; }

        object Subject { get; }
    }
}