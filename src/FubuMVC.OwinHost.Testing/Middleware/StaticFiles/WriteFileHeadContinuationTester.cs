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
    public class WriteFileHeadContinuation_should_only_write_the_content_type_if_that_status_is_200
    {
        private FubuFile theFile;
        private IHttpWriter theWriter;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().WriteStringToFile("foo.txt", "some text");
            theFile = new FubuFile("foo.txt", "application");

            theWriter = MockRepository.GenerateMock<IHttpWriter>();
            
        }

        [Test]
        public void do_write_content_length_for_200()
        {
            new WriteFileHeadContinuation(theWriter, theFile, HttpStatusCode.OK)
                .Write(theWriter);

            theWriter.AssertWasCalled(x => x.AppendHeader(HttpResponseHeaders.ContentLength, theFile.Length().ToString()));
        }

        [Test]
        public void do_not_write_lenght_for_anything_but_200()
        {
            new WriteFileHeadContinuation(theWriter, theFile, HttpStatusCode.SeeOther)
                .Write(theWriter);

            theWriter.AssertWasNotCalled(x => x.AppendHeader(HttpResponseHeaders.ContentLength, theFile.Length().ToString()));
        }
    }


    [TestFixture]
    public class when_writing_the_file_headers
    {
        private FubuFile theFile;
        private IHttpWriter theWriter;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().WriteStringToFile("foo.txt", "some text");
            theFile = new FubuFile("foo.txt", "application");

            theWriter = MockRepository.GenerateMock<IHttpWriter>();

            WriteFileHeadContinuation.WriteHeaders(theWriter, theFile);
        }

        private void assertHeaderValueWasWritten(string key, string value)
        {
            theWriter.AssertWasCalled(x => x.AppendHeader(key, value));
        }

        [Test]
        public void should_write_the_content_type()
        {
            assertHeaderValueWasWritten(HttpResponseHeaders.ContentType, "text/plain");
        }

        [Test]
        public void should_write_the_last_modified_header()
        {
            assertHeaderValueWasWritten(HttpResponseHeaders.LastModified, theFile.LastModified().ToString("r"));
        }

        [Test]
        public void should_write_the_quoted_etag()
        {
            assertHeaderValueWasWritten(HttpResponseHeaders.ETag, theFile.Etag().Quoted());
        }
    }
}