using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
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
            ClassUnderTest.RedirectToUrl("some url");
            MockFor<IDebugReport>().AddDetails(new RedirectReport
            {
                Url = "some url"
            });
        }

        [Test]
        public void write_content()
        {
            ClassUnderTest.Write(MimeType.Json.ToString(), "some output");
            MockFor<IDebugReport>().AssertWasCalled(x => x.AddDetails(new OutputReport
            {
                Contents = "some output",
                ContentType = MimeType.Json.ToString()
            }));
        }

        [Test]
        public void write_file()
        {
            ClassUnderTest.WriteFile(MimeType.Html.ToString(), "local file path", "display name");
            MockFor<IDebugReport>().AssertWasCalled(x => x.AddDetails(new FileOutputReport
            {
                ContentType = MimeType.Html.ToString(),
                DisplayName = "display name",
                LocalFilePath = "local file path"
            }));
        }
    }
}