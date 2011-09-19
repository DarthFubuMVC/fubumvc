using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Rest.Media
{
    public interface IValues<T>
    {
        object ValueFor(Accessor accessor);

        T Subject { get; }
    }

    public interface IValueStream<T>
    {
        IEnumerable<IValues<T>> Elements { get;}
    }
}