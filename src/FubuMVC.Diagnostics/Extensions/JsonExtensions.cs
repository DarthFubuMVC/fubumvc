using FubuMVC.Diagnostics.Infrastructure;

namespace FubuMVC.Diagnostics.Extensions
{
    public static class JsonExtensions
    {
        public static string AsJson(this object target)
        {
            return JsonService.Serialize(target);
        }
    }
}