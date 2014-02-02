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
        private IHttpWriter theWriter;
        private FubuFile theFile;

        [SetUp]
        public void SetUp()
        {
            theWriter = MockRepository.GenerateMock<IHttpWriter>();

            new FileSystem().WriteStringToFile("foo.txt", "some text");
            theFile = new FubuFile("foo.txt", "application");

            new WriteFileContinuation(theWriter, theFile)
                .Write(theWriter);
        }


        private void assertHeaderValueWasWritten(string key, string value)
        {
            theWriter.AssertWasCalled(x => x.AppendHeader(key, value));
        }


        [Test]
        public void should_write_ok_as_the_status_code()
        {
            theWriter.AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.OK));
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
            theWriter.AssertWasCalled(x => x.WriteFile(theFile.Path));
        }
    }
}