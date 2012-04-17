using System.Collections.Generic;

namespace FubuMVC.Media.Projections
{
    public interface IValueStream<T>
    {
        IEnumerable<IValues<T>> Elements { get; }
    }
}