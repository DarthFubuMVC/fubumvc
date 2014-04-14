using FubuCore.Reflection;

namespace FubuMVC.Core.Projections
{
    public interface IValues<T>
    {
        T Subject { get; }
        object ValueFor(Accessor accessor);
        
    }

}