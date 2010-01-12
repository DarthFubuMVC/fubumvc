namespace FubuMVC.Core.Runtime
{
    public interface IOutputWriter
    {
        void WriteFile(string contentType, string localFilePath, string displayName);
        void Write(string contentType, string renderedOutput);
        void RedirectToUrl(string url);
    }
}