using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public interface IRecordedOutput
    {
        void Replay(IHttpWriter writer);
    }
}