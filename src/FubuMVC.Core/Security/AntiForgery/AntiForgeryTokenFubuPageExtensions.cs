using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.Security.AntiForgery
{
    public static class AntiForgeryTokenFubuPageExtensions
    {
        public static HtmlTag AntiForgeryToken(this IFubuPage page, string salt)
        {
            var settings = page.Get<AntiForgerySettings>();
            return AntiForgeryToken(page, salt, settings.Path, settings.Domain);
        }

        public static HtmlTag AntiForgeryToken(this IFubuPage page, string salt, string path, string domain)
        {
            var antiForgeryService = page.Get<IAntiForgeryService>();
            AntiForgeryData cookieToken = antiForgeryService.SetCookieToken(path, domain);
            FormToken formToken = antiForgeryService.GetFormToken(cookieToken, salt);

            return new HiddenTag().Name(formToken.Name).Value(formToken.TokenString);
        }
    }
}
