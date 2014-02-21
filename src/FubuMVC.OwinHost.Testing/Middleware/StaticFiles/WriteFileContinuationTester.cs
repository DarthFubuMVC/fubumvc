using System.Net;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.OwinHost.Middleware.StaticFiles;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.OwinHost.Testing.Middleware.StaticFiles
{
    [TestFixture]
    public class when_writing_a_file
    {
        private IHttpResponse theResponse;
        private FubuFile theFile;

        [SetUp]
        public void SetUp()
        {
            theResponse = MockRepository.GenerateMock<IHttpResponse>();

            new FileSystem().WriteStringToFile("foo.txt", "some text");
            theFile = new FubuFile("foo.txt", "application");

            new WriteFileContinuation(theResponse, theFile)
                .Write(theResponse);
        }


        private void assertHeaderValueWasWritten(string key, string value)
        {
            theResponse.AssertWasCalled(x => x.AppendHeader(key, value));
        }


        [Test]
        public void should_write_ok_as_the_status_code()
        {
            theResponse.AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.OK));
        }

        [Test]
        public void should_append_the_normal_file_headers()
        {
            // I'm saying this is enough to prove it's delegating
            assertHeaderValueWasWritten(HttpResponseHeaders.ETag, theFile.Etag().Quoted());
        }

        [Test]
        public void should_write_The_file_itself()
        {
            theResponse.AssertWasCalled(x => x.WriteFile(theFile.Path));
        }
    }
}