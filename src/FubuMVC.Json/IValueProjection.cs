using FubuCore.Util;

namespace FubuMVC.Json
{
    public interface IValueProjection<T>
    {
        void Map(T target, Cache<string, object> props);
    }
}