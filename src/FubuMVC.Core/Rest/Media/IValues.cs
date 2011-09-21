using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Rest.Media
{
    public interface IValues<T>
    {
        T Subject { get; }
        object ValueFor(Accessor accessor);
    }

    public interface IValueStream<T>
    {
        IEnumerable<IValues<T>> Elements { get; }
    }
}