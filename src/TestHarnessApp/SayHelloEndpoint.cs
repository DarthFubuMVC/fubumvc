using FubuMVC.Core.Http;

namespace TestHarnessApp
{
    public class SayHelloEndpoint
    {
        private readonly IHttpRequest _request;

        public SayHelloEndpoint(IHttpRequest request)
        {
            _request = request;
        }

        public string get_hello()
        {
            var name = _request.QueryString["name"];

            return "Hello, " + name;
        } 
    }
}