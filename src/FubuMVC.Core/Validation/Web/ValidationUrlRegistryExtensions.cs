using FubuMVC.Core.Urls;
using FubuMVC.Core.Validation.Web.Remote;

namespace FubuMVC.Core.Validation.Web
{
    public static class ValidationUrlRegistryExtensions
    {
        public static string RemoteRule(this IUrlRegistry urls)
        {
            return urls.UrlFor(new ValidateField());
        }
    }
}