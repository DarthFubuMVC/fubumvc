using System;
using System.Net;
using System.Web;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{

    [TestFixture]
    public class DebuggingOutputWriterTester_during_non_debug_call : InteractionContext<RecordingOutputWriter>
    {
        [Test]
        public void should_use_normal_http_output_writer_for_non_debug_calls()
        {
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(false);
            ClassUnderTest.Writer.ShouldBeTheSameAs(MockFor<IHttpWriter>());
        }
    }

    [TestFixture]
    public class DebuggingOutputWriterTester_during_debug_call : InteractionContext<RecordingOutputWriter>
    {
        [Test]
        public void should_use_http_response_output_writer_for_non_debug_calls()
        {
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(true);
            ClassUnderTest.Writer.ShouldBeOfType<NulloHttpWriter>();
        }
    }

    [TestFixture]
    public class RecordingOutputWriterTester : InteractionContext<RecordingOutputWriter>
    {
        protected override void beforeEach()
        {
            MockFor<IDebugDetector>().Stub(x => x.IsOutputWritingLatched()).Return(false);

        }


        [Test]
        public void redirect_to_url()
        {
        	var url = "some url";
            ClassUnderTest.RedirectToUrl(url);

            

        	MockFor<IDebugReport>().AddDetails(new RedirectReport
        	                                   	{
        	                                   		Url = url
        	                                   	});
			MockFor<IHttpWriter>()
				.AssertWasCalled(w => w.Redirect(url));
        }

        [Test]
        public void write_content()
        {
            ClassUnderTest.Write(MimeType.Json.ToString(), "some output");
        	MockFor<IDebugReport>()
        		.AssertWasCalled(x => x.AddDetails(new OutputReport
        		                                   	{
        		                                   		Contents = "some output",
        		                                   		ContentType = MimeType.Json.ToString()
        		                                   	}));
			MockFor<IHttpWriter>()
				.AssertWasCalled(w => w.Write("some output"));

            MockFor<IHttpWriter>()
                .AssertWasCalled(w => w.WriteContentType(MimeType.Json.ToString()));
        }

        [Test]
        public void write_file()
        {
            ClassUnderTest.WriteFile(MimeType.Html.ToString(), "local file path", "display name");
        	MockFor<IDebugReport>()
        		.AssertWasCalled(x => x.AddDetails(new FileOutputReport
        		                                   	{
        		                                   		ContentType = MimeType.Html.ToString(),
        		                                   		DisplayName = "display name",
        		                                   		LocalFilePath = "local file path"
        		                                   	}));
        	MockFor<IHttpWriter>()
        		.AssertWasCalled(w => w.WriteFile("local file path"));
        }

		[Test]
		public void write_response_code()
		{
			ClassUnderTest.WriteResponseCode(HttpStatusCode.Unauthorized);
			MockFor<IDebugReport>()
				.AssertWasCalled(x => x.AddDetails(new HttpStatusReport { Status = HttpStatusCode.Unauthorized }));
			
            MockFor<IHttpWriter>()
				.AssertWasCalled(w => w.WriteResponseCode(HttpStatusCode.Unauthorized));
		}

		[Test]
		public void write_cookie()
		{
			var cookie = new HttpCookie("Some Cookie");
			
			ClassUnderTest
				.AppendCookie(cookie);

			MockFor<IHttpWriter>()
				.AssertWasCalled(w => w.AppendCookie(cookie));
		}

        //[Test]
        //public void recorded_output()
        //{
        //    Action action = () => { };
        //    var theRecordedOutput = new RecordedOutput("content type", "the output");

        //    MockFor<IOutputWriter>().Stub(x => x.Record(action)).Return(theRecordedOutput);

        //    ClassUnderTest.Record(action);

        //    MockFor<IDebugReport>().AssertWasCalled(x => x.AddDetails(new OutputReport(){
        //        Contents = theRecordedOutput.Content,
        //        ContentType = theRecordedOutput.ContentType
        //    }));
        //}
    }
}