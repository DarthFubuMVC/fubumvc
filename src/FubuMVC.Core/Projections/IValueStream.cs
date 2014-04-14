using System.Collections.Generic;

namespace FubuMVC.Core.Projections
{
    public interface IValueStream<T>
    {
        IEnumerable<IValues<T>> Elements { get; }
    }
}