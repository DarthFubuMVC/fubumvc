using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;

namespace FubuMVC.Core.Diagnostics
{
    public class HttpRequestSummary
    {
        // For serialization
        public HttpRequestSummary()
        {
            type = "http";
        }

        public HttpRequestSummary(ChainExecutionLog log) : this()
        {
            var request = new OwinHttpRequest(log.Request);
            var response = new OwinHttpResponse(log.Request);

            id = log.Id.ToString();
            time = log.Time.ToHttpDateString();
            url = request.RawUrl();
            method = request.HttpMethod();
            status = response.StatusCode;
            description = response.StatusDescription;
            if (status == 302)
            {
                // TODO -- write a helper for location
                description = response.HeaderValueFor(HttpResponseHeaders.Location).SingleOrDefault();
            }

            contentType = response.ContentType();
            duration = log.ExecutionTime;
        }

        public string type { get; set; }

        public string id { get; set; }
        public string time { get; set; }
        public string url { get; set; }
        public string method { get; set; }
        public int status { get; set; }
        public string description { get; set; }
        public string contentType { get; set; }
        public double duration { get; set; }
    }
}