using FubuCore.Util;

namespace FubuMVC.Json
{
    public interface IProjection
    {
        Cache<string, object> Flatten(object target);
    }
}