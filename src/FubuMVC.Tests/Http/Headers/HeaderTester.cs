using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Headers
{
    
    public class HeaderTester
    {
        [Fact]
        public void replay_writes_to_the_IHttpWriter()
        {
            var header = new Header(HttpResponseHeader.Warning, "don't do it!");

            var writer = MockRepository.GenerateMock<IHttpResponse>();

            header.Replay(writer);

            writer.AssertWasCalled(x => x.AppendHeader("Warning", "don't do it!"));
        }
    }
}