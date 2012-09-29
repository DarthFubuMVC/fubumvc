using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Core.UI
{
    public static class AntiForgeryTokenFubuPageExtensions
    {
        public static HtmlTag AntiForgeryToken(this IFubuPage page, string salt)
        {
            return AntiForgeryToken(page, salt, null, null);
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