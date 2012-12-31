using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinStreamingData : IStreamingData
    {
        private readonly Stream _stream;

        public OwinStreamingData(IDictionary<string, object> environment)
        {
            _stream = environment.Get<Stream>(OwinConstants.RequestBodyKey);
        }

        public Stream Input
        {
            get
            {
                return _stream;
            }
        }
    }
}