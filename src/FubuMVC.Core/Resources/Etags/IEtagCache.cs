namespace FubuMVC.Core.Resources.Etags
{
    public interface IEtagCache
    {
        // Can be null
        string Current(string resourceHash);
        void Register(string resourceHash, string etag);
        void Eject(string resourceHash);
    }
}