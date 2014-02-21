using System.IO;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public interface IRecordedHttpOutput
    {
        void Replay(IHttpResponse response);
    }

    public interface IRecordedTextOutput
    {
        void WriteText(StringWriter writer);
    }
}