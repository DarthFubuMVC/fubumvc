using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public interface IRecordedHttpOutput
    {
        void Replay(IHttpWriter writer);
    }
}