using System;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Caching
{
    public interface IRecordedOutput
    {
        void Replay(IHttpWriter writer);

        string GetText();




        void ForHeader(string headerName, Action<string> action);
        string GetHeaderValue(string headerName);
    }
}