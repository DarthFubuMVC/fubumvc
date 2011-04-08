using System.Collections.Generic;

namespace FubuMVC.Json
{
    public interface IProjectionSource
    {
        IEnumerable<IValueProjection<T>> ValueProjectionsFor<T>();
        IEnumerable<IValueProjection<T>> ValueProjectionsFor<T>(string name);
    }
}