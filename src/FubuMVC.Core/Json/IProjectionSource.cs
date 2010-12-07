using System.Collections.Generic;

namespace FubuMVC.Core.Json
{
    public interface IProjectionSource
    {
        IEnumerable<IValueProjection<T>> ValueProjectionsFor<T>();
        IEnumerable<IValueProjection<T>> ValueProjectionsFor<T>(string name);
    }
}