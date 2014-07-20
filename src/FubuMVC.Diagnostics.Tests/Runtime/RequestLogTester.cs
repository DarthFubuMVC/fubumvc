using System;
using System.Net;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Logging;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Runtime
{
    [TestFixture]
    public class RequestLogTester
    {
        [Test]
        public void add_log()
        {
            var log = new RequestLog();

            var stringMessage = new StringMessage("something");
            log.AddLog(123.45, stringMessage);

            log.AddLog(234, "something");

            log.AllSteps()
                .ShouldHaveTheSameElementsAs(new RequestStep(123.45, stringMessage), new RequestStep(234, "something"));
        }

        [Test]
        public void failed_is_false_by_default()
        {
            // trivial code, but kind of important
            new RequestLog().Failed.ShouldBeFalse();
        }

        [Test]
        public void has_a_unique_id()
        {
            var log1 = new RequestLog();
            var log2 = new RequestLog();
            var log3 = new RequestLog();

            log1.Id.ShouldNotEqual(Guid.Empty);
            log2.Id.ShouldNotEqual(Guid.Empty);
            log3.Id.ShouldNotEqual(Guid.Empty);

            log1.Id.ShouldNotEqual(log2.Id);
            log1.Id.ShouldNotEqual(log3.Id);
            log2.Id.ShouldNotEqual(log3.Id);
        }


        [Test]
        public void content_type_with_out_any_headers()
        {
            var log = new RequestLog();
            log.ContentType.ShouldEqual("Unknown");
        }

        [Test]
        public void content_type_with_no_content_type_header()
        {
            var log = new RequestLog();
            log.ResponseHeaders = new Header[0];

            log.ContentType.ShouldEqual("Unknown");
        }

        [Test]
        public void content_type_with_a_content_type_header()
        {
            var log = new RequestLog();
            log.ResponseHeaders = new Header[]{new Header(HttpResponseHeader.ContentType, MimeType.Javascript.Value)};

            log.ContentType.ShouldEqual(MimeType.Javascript.Value);
        }
    }

    public class Fake
    {
        public string Name { get; set; }
    }
}