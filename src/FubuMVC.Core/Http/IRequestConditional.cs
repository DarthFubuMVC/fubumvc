namespace FubuMVC.Core.Http
{
    public interface IRequestConditional
    {
        bool Matches(string mimeType);
    }
}