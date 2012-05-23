using System;
using System.Net;
using FubuCore.Binding.InMemory;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class HeaderValueAttributeTester
    {
        [Test]
        public void bind_by_header()
        {
            var headers = new StubRequestHeaders();
            headers.Data["Last-Event-ID"] = "something";
            headers.Data[HttpResponseHeaders.Warning] = "oh no!";

            var target = BindingScenario<HeaderValueTarget>.For(x =>
            {
                x.Service<IRequestHeaders>(headers);


            }).Model;

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