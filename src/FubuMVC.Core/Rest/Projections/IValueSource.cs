using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Rest.Projections
{
    public interface IValueSource<T>
    {
        object ValueFor(Accessor accessor);

        T Subject { get; }
    }

    public interface IValueStream<T>
    {
        IEnumerable<IValueSource<T>> Elements { get;}
    }
}