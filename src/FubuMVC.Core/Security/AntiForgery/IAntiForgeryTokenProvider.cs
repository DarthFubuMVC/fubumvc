namespace FubuMVC.Core.Security.AntiForgery
{
    public interface IAntiForgeryTokenProvider
    {
        string GetTokenName();
        string GetTokenName(string appPath);
        AntiForgeryData GenerateToken();
    }
}