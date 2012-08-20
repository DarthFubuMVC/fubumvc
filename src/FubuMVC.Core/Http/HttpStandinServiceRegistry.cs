using System;
using System.IO;
using FubuCore.Binding;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    public class HttpStandInServiceRegistry : ServiceRegistry
    {
        public HttpStandInServiceRegistry()
        {
            SetServiceIfNone<ICurrentHttpRequest, StandInCurrentHttpRequest>();

            SetServiceIfNone<IRequestHeaders, RequestHeaders>();
            SetServiceIfNone<IRequestData>(new RequestData());
            SetServiceIfNone<IClientConnectivity, StandInClientConnectivity>();

            SetServiceIfNone<ICookies, InMemoryCookies>();

            SetServiceIfNone<IHttpWriter, NulloHttpWriter>();

            SetServiceIfNone<ICurrentChain>(new CurrentChain(null, null));
            SetServiceIfNone<IStreamingData, NulloStreamingData>();
        }

        public class NulloStreamingData : IStreamingData
        {
            public Stream Input
            {
                get { return new MemoryStream(); }
            }
        }
    }
}