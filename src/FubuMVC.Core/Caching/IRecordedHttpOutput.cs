using System.IO;
using System.Threading.Tasks;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public interface IRecordedHttpOutput
    {
        Task Replay(IHttpResponse response);
    }

    public interface IRecordedTextOutput
    {
        void WriteText(StringWriter writer);
    }
}