using FubuCore.Util;

namespace FubuMVC.Core.Json
{
    public interface IValueProjection<T>
    {
        void Map(T target, Cache<string, object> props);
    }
}