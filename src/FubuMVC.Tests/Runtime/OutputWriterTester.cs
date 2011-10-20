using System;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class OutputWriterTester : InteractionContext<OutputWriter>
    {
        private IHttpWriter theHttpWriter;

        protected override void beforeEach()
        {
            theHttpWriter = MockFor<IHttpWriter>();
        }

        [Test]
        public void write_in_normal_mode_delegates_to_http_writer()
        {
            ClassUnderTest.Write("text/json", "{}");

            theHttpWriter.AssertWasCalled(x => x.Write("{}"));
            theHttpWriter.AssertWasCalled(x => x.WriteContentType("text/json"));
        }

        [Test]
        public void redirect_to_a_url_delegates()
        {
            ClassUnderTest.RedirectToUrl("http://somewhere.com");

            theHttpWriter.AssertWasCalled(x => x.Redirect("http://somewhere.com"));
        }

        [Test]
        public void write_response_code_delegates()
        {
            ClassUnderTest.WriteResponseCode(HttpStatusCode.UseProxy);

            theHttpWriter.AssertWasCalled(x => x.WriteResponseCode(HttpStatusCode.UseProxy));
        }
    }

    [TestFixture]
    public class when_writing_within_recorded_output_mode : InteractionContext<OutputWriter>
    {
        private string theContent;
        private string theContentType;
        private RecordedOutput theRecordedOutput;

        protected override void beforeEach()
        {
            theContent = "some content";
            theContentType = "text/xml";

            theRecordedOutput = ClassUnderTest.Record(() =>
            {
                ClassUnderTest.Write(theContentType, theContent);
            });
        }

        [Test]
        public void should_not_have_written_directly_to_the_http_writer()
        {
            MockFor<IHttpWriter>().AssertWasNotCalled(x => x.Write(theContent));
            MockFor<IHttpWriter>().AssertWasNotCalled(x => x.WriteContentType(theContentType));
        }

        [Test]
        public void recorded_output_should_have_what_was_written()
        {
            theRecordedOutput.Content.ShouldEqual(theContent);
            theRecordedOutput.ContentType.ShouldEqual(theContentType);
        }
    }

    [TestFixture]
    public class when_writing_a_file_with_a_display_name : InteractionContext<OutputWriter>
    {
        private string theDisplayName;
        private string theFilePath;
        private string theContentType;

        protected override void beforeEach()
        {
            theDisplayName = "The title";
            theFilePath = "location";
            theContentType = "content type";

            MockFor<IFileSystem>().Stub(x => x.FileSizeOf(theFilePath)).Return(123);

            ClassUnderTest.WriteFile(theContentType, theFilePath, theDisplayName);
        }

        [Test]
        public void should_have_written_the_content_type()
        {
            MockFor<IHttpWriter>().AssertWasCalled(x => x.WriteContentType(theContentType));
        }

        [Test]
        public void should_write_a_content_disposition_header_for_the_display()
        {
            MockFor<IHttpWriter>().AssertWasCalled(x => x.AppendHeader("Content-Disposition", "attachment; filename=\"The title\""));
        }

        [Test]
        public void should_write_header_for_content_length()
        {
            MockFor<IHttpWriter>().AssertWasCalled(x => x.AppendHeader("Content-Length", "123"));
        }

        [Test]
        public void should_actually_you_know_write_the_file_itself()
        {
            MockFor<IHttpWriter>().AssertWasCalled(x => x.WriteFile(theFilePath));
        }
    }
}