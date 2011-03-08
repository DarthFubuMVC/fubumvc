namespace FubuMVC.Core.Security.AntiForgery
{
    public interface IAntiForgeryValidator
    {
        bool Validate(string salt);
    }
}