namespace FubuMVC.Core.Http.Cookies
{
    public interface ICookieValue
    {
        string Value { get; set; }
        void Erase();
    }
}