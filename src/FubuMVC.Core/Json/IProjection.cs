using FubuCore.Util;

namespace FubuMVC.Core.Json
{
    public interface IProjection
    {
        Cache<string, object> Flatten(object target);
    }
}