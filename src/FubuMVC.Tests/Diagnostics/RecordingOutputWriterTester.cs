using System.Net;
using System.Web;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class RecordingOutputWriterTester : InteractionContext<RecordingOutputWriter>
    {
        [Test]
        public void redirect_to_url()
        {
        	var url = "some url";
            ClassUnderTest.RedirectToUrl(url);
        	MockFor<IDebugReport>().AddDetails(new RedirectReport
        	                                   	{
        	                                   		Url = url
        	                                   	});
			MockFor<IOutputWriter>()
				.AssertWasCalled(w => w.RedirectToUrl(url));
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
			MockFor<IOutputWriter>()
				.AssertWasCalled(w => w.Write(MimeType.Json.ToString(), "some output"));
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
        	MockFor<IOutputWriter>()
        		.AssertWasCalled(w => w.WriteFile(MimeType.Html.ToString(), "local file path", "display name"));
        }

		[Test]
		public void write_response_code()
		{
			ClassUnderTest.WriteResponseCode(HttpStatusCode.Unauthorized);
			MockFor<IDebugReport>()
				.AssertWasCalled(x => x.AddDetails(new HttpStatusReport { Status = HttpStatusCode.Unauthorized }));
			MockFor<IOutputWriter>()
				.AssertWasCalled(w => w.WriteResponseCode(HttpStatusCode.Unauthorized));
		}

		[Test]
		public void write_cookie()
		{
			var cookie = new HttpCookie("Some Cookie");
			
			ClassUnderTest
				.AppendCookie(cookie);

			MockFor<IOutputWriter>()
				.AssertWasCalled(w => w.AppendCookie(cookie));
		}
    }
}