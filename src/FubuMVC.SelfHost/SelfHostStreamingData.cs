using System.IO;
using System.Net.Http;
using FubuMVC.Core.Http;

namespace FubuMVC.SelfHost
{
    public class SelfHostStreamingData : IStreamingData
    {
        private readonly HttpRequestMessage _request;

        public SelfHostStreamingData(HttpRequestMessage request)
        {
            _request = request;
        }

        public Stream Input
        {
            // TODO -- opportunity to make things faster
            get { return _request.Content.ReadAsStreamAsync().Result; }
        }
    }
}