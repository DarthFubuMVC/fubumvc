using System.Collections.Generic;

namespace FubuMVC.Core.Resources.Media
{
    public interface IValueStream<T>
    {
        IEnumerable<IValues<T>> Elements { get; }
    }
}