using System.Net;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Runtime.Files;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Owin.Middleware.StaticFiles
{
    
    public class when_writing_a_file
    {
        private IHttpResponse theResponse;
        private FubuFile theFile;

        public when_writing_a_file()
        {
            theResponse = MockRepository.GenerateMock<IHttpResponse>();

            new FileSystem().WriteStringToFile("foo.txt", "some text");
            theFile = new FubuFile("foo.txt");

            new WriteFileContinuation(theResponse, theFile, new AssetSettings())
                .Write(theResponse);
        }


        private void assertHeaderValueWasWritten(string key, string value)
        {
            theResponse.AssertWasCalled(x => x.AppendHeader(key, value));
        }


        [Fact]
        public void should_write_ok_as_the_status_code()
        {
            theResponse.AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.OK));
        }

        [Fact]
        public void should_append_the_normal_file_headers()
        {
            // I'm saying this is enough to prove it's delegating
            assertHeaderValueWasWritten(HttpResponseHeaders.ETag, theFile.Etag().Quoted());
        }

        [Fact]
        public void should_write_The_file_itself()
        {
            theResponse.AssertWasCalled(x => x.WriteFile(theFile.Path));
        }
    }
}