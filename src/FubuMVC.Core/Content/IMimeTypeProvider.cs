namespace FubuMVC.Core.Content
{
    public interface IMimeTypeProvider
    {
        string For(string extension);
        void Register(string extension, string mimeType);
    }
}