namespace FubuMVC.AntiForgery
{
    public interface IAntiForgeryTokenProvider
    {
        string GetTokenName();
        string GetTokenName(string appPath);
        AntiForgeryData GenerateToken();
    }
}