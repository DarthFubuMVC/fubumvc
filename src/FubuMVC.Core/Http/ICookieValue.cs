namespace FubuMVC.Core.Http
{
    public interface ICookieValue
    {
        string Value { get; set; }
        void Erase();
    }
}