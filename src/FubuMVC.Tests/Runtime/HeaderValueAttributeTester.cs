using System.Net;
using FubuCore.Binding.InMemory;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class HeaderValueAttributeTester
    {
        [Test]
        public void bind_by_header()
        {
            var headers = OwinHttpRequest.ForTesting();
            headers.Header("Last-Event-ID", "something");
            headers.Header(HttpResponseHeaders.Warning, "oh no!");


            var target = BindingScenario<HeaderValueTarget>.For(x => { x.Service<IHttpRequest>(headers); }).Model;

            target.LastEventId.ShouldEqual("something");
            target.Warning.ShouldEqual("oh no!");
        }
    }


    public class HeaderValueTarget
    {
        [HeaderValue("Last-Event-ID")]
        public string LastEventId { get; set; }

        [HeaderValue(HttpRequestHeader.Warning)]
        public string Warning { get; set; }
    }
}