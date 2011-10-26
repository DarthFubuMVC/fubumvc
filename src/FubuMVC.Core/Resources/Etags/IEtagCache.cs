namespace FubuMVC.Core.Resources.Etags
{
    public interface IEtagCache
    {
        // Can be null
        string CurrentETag(string resourcePath);
        void WriteCurrentETag(string resourcePath, string etag);
    }
}