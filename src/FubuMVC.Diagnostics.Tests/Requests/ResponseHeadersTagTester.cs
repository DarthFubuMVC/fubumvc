using System.Linq;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Diagnostics.Requests;
using FubuMVC.Diagnostics.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Requests
{
    [TestFixture]
    public class ResponseHeadersTagTester
    {
        [Test]
        public void do_not_display_if_no_headers()
        {
            var log = new RequestLog();
            log.ResponseHeaders.Any().ShouldBeFalse();

            new ResponseHeadersTag(log).Render().ShouldBeFalse();
        }

        [Test]
        public void does_display_if_there_are_response_headers()
        {
            var log = new RequestLog();
            log.ResponseHeaders = new Header[]{new Header("a", "1"), new Header("b", "2"), new Header("c", "3")};

            new ResponseHeadersTag(log).Render().ShouldBeTrue();
        }
    }
}