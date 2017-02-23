using System.Net;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Runtime.Files;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Owin.Middleware.StaticFiles
{
    
    public class WriteFileHeadContinuation_should_only_write_the_content_type_if_that_status_is_200
    {
        private FubuFile theFile;
        private IHttpResponse theResponse;

        public WriteFileHeadContinuation_should_only_write_the_content_type_if_that_status_is_200()
        {
            new FileSystem().WriteStringToFile("foo.txt", "some text");
            theFile = new FubuFile("foo.txt");

            theResponse = MockRepository.GenerateMock<IHttpResponse>();
            
        }

        [Fact]
        public void do_write_content_length_for_200()
        {
            new WriteFileHeadContinuation(theResponse, theFile, HttpStatusCode.OK)
                .Write(theResponse);

            theResponse.AssertWasCalled(x => x.AppendHeader(HttpResponseHeaders.ContentLength, theFile.Length().ToString()));
        }

        [Fact]
        public void do_not_write_lenght_for_anything_but_200()
        {
            new WriteFileHeadContinuation(theResponse, theFile, HttpStatusCode.SeeOther)
                .Write(theResponse);

            theResponse.AssertWasNotCalled(x => x.AppendHeader(HttpResponseHeaders.ContentLength, theFile.Length().ToString()));
        }
    }


    
    public class when_writing_the_file_headers
    {
        private FubuFile theFile;
        private IHttpResponse theResponse;

        public when_writing_the_file_headers()
        {
            new FileSystem().WriteStringToFile("foo.txt", "some text");
            theFile = new FubuFile("foo.txt");

            theResponse = MockRepository.GenerateMock<IHttpResponse>();

            WriteFileHeadContinuation.WriteHeaders(theResponse, theFile);
        }

        private void assertHeaderValueWasWritten(string key, string value)
        {
            theResponse.AssertWasCalled(x => x.AppendHeader(key, value));
        }

        [Fact]
        public void should_write_the_content_type()
        {
            assertHeaderValueWasWritten(HttpResponseHeaders.ContentType, "text/plain");
        }

        [Fact]
        public void should_write_the_last_modified_header()
        {
            assertHeaderValueWasWritten(HttpResponseHeaders.LastModified, theFile.LastModified().ToString("r"));
        }

        [Fact]
        public void should_write_the_quoted_etag()
        {
            assertHeaderValueWasWritten(HttpResponseHeaders.ETag, theFile.Etag().Quoted());
        }
    }
}