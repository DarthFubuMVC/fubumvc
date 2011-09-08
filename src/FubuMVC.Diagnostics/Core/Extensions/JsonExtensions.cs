using FubuMVC.Diagnostics.Core.Infrastructure;

namespace FubuMVC.Diagnostics.Core.Extensions
{
    public static class JsonExtensions
    {
        public static string AsJson(this object target)
        {
            return JsonService.Serialize(target);
        }
    }
}