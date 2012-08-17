using System;
using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Caching
{
    public interface IRecordedOutput
    {
        void Replay(IHttpWriter writer);

        string GetText();

        IEnumerable<Header> Headers();
    }
}