using System.Collections.Generic;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class HttpRequestSummaryTester
    {
        [Test]
        public void map_from_execution_log_happy_path()
        {
            var log = new ChainExecutionLog();
            log.Request.Add(OwinConstants.ResponseStatusCodeKey, 200);
            log.Request.ResponseHeaders().Add(HttpResponseHeaders.ContentType, new []{"text/plain"});

            var request = new OwinHttpRequest(log.Request);
            request.HttpMethod("POST");
            request.FullUrl("http://server/foo");

            log.MarkFinished();

            var summary = new HttpRequestSummary(log);

            summary.id.ShouldBe(log.Id.ToString());
            summary.time.ShouldBe(log.Time.ToHttpDateString());
            summary.url.ShouldBe("/foo");
            summary.method.ShouldBe("POST");
            summary.status.ShouldBe(200);
            summary.contentType.ShouldBe("text/plain");
            summary.duration.ShouldBe(log.ExecutionTime);
        }

        [Test]
        public void map_from_execution_when_the_request_was_redirected()
        {
            var log = new ChainExecutionLog();
            log.Request.Add(OwinConstants.ResponseStatusCodeKey, 302);
            log.Request.ResponseHeaders().Add(HttpResponseHeaders.ContentType, new[] { "text/plain" });
            log.Request.ResponseHeaders().Add(HttpResponseHeaders.Location, new[] { "/new/place" });


            var request = new OwinHttpRequest(log.Request);
            request.HttpMethod("GET");
            request.FullUrl("http://server/foo");

            log.MarkFinished();

            var summary = new HttpRequestSummary(log);

            summary.id.ShouldBe(log.Id.ToString());
            summary.time.ShouldBe(log.Time.ToHttpDateString());
            summary.url.ShouldBe("/foo");
            summary.method.ShouldBe("GET");
            summary.status.ShouldBe(302);
            summary.contentType.ShouldBe("text/plain");
            summary.duration.ShouldBe(log.ExecutionTime);

            summary.description.ShouldBe("/new/place");
        }
    }
}