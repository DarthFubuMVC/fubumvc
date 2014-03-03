using System.Net;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Owin.Middleware.StaticFiles
{
    [TestFixture]
    public class WriteStatusCodeContinuationTester
    {
        [Test]
        public void writes_the_status_code_and_reason()
        {
            var writer = MockRepository.GenerateMock<IHttpResponse>();


            var continuation = new WriteStatusCodeContinuation(writer, HttpStatusCode.Accepted, "it's all good");

            continuation.Write(writer);

            writer.AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.Accepted, "it's all good"));
        }
    }
}