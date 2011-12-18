using FubuCore.Reflection;

namespace FubuMVC.Core.Resources.Media
{
    public interface IValues<T>
    {
        T Subject { get; }
        object ValueFor(Accessor accessor);
    }
}