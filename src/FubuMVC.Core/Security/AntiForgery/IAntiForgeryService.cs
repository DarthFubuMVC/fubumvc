namespace FubuMVC.Core.Security.AntiForgery
{
    public interface IAntiForgeryService
    {
        AntiForgeryData SetCookieToken(string path, string domain);
        FormToken GetFormToken(AntiForgeryData token, string salt);
    }
}