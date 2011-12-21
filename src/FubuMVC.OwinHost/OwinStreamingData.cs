using System;
using System.IO;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinStreamingData : IStreamingData
    {
        private readonly MemoryStream _stream;

        public OwinStreamingData(OwinRequestBody request)
        {
            _stream = request.Stream ?? new MemoryStream();
        }

        public Stream Input
        {
            get
            {
                _stream.Position = 0;
                return _stream;
            }
        }
    }
}