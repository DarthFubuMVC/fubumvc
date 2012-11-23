using System.Net;
using FubuMVC.Core.Runtime;

namespace AspNetApplication
{
    public class StatusCodeEndpoint
    {
        private readonly IOutputWriter _writer;

        public StatusCodeEndpoint(IOutputWriter writer)
        {
            _writer = writer;
        }

        public string get_not_modified()
        {
            _writer.WriteResponseCode(HttpStatusCode.NotModified, "No changes here");

            return "Nothing to see here";
        }
    }
}