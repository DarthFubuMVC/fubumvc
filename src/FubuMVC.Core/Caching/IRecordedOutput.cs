using System;
using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Caching
{
    public interface IRecordedOutput : IHaveContentType
    {
        void Replay(IHttpResponse response);

        string GetText();

        IEnumerable<Header> Headers();

        bool IsEmpty();

        IEnumerable<IRecordedHttpOutput> Outputs { get; }
    }
}