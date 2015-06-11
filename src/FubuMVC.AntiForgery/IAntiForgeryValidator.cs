namespace FubuMVC.AntiForgery
{
    public interface IAntiForgeryValidator
    {
        bool Validate(string salt);
    }
}