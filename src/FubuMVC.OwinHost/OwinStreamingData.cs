using System;
using System.IO;
using FubuMVC.Core.Http;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinStreamingData : IStreamingData
    {
        private readonly Request _request;
        private readonly Response _response;

        public OwinStreamingData(Request request, Response response)
        {
            _request = request;
            _response = response;
        }

        public Stream Input
        {
            get { return new InputStream(_request.Body); }
        }

        public Stream Output
        {
            get
            {
                Action complete = () => { };
                return new OutputStream((segment, continuation) =>
                {
                    _response.BinaryWrite(segment);
                    return true;
                }, complete);
            }
        }

        public string OutputContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }
    }
}