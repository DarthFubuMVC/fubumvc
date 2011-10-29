using System;
using System.IO;
using FubuMVC.Core.Http;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinStreamingData : IStreamingData
    {
        private readonly Request _request;

        public OwinStreamingData(Request request)
        {
            _request = request;
        }

        public Stream Input
        {
            get { return new InputStream(_request.Body); }
        }
    }
}