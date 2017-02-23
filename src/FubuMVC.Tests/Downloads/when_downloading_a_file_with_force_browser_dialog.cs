using FubuMVC.Core.Downloads;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Downloads
{
    
    public class when_downloading_a_file_with_force_browser_dialog : InteractionContext<DownloadFileBehavior>
    {
        private DownloadFileModel _output;

        protected override void beforeEach()
        {
            _output = new DownloadFileModel
            {
                ContentType = "image/jpeg",
                FileNameToDisplay = "displayname",
                LocalFileName = "filename.jpg"
            };

            MockFor<IFubuRequest>().Expect(x => x.Get<DownloadFileModel>()).Return(_output);

            ClassUnderTest.Invoke();
        }

        [Fact]
        public void should_set_content_type_correctly()
        {
            MockFor<IOutputWriter>().AssertWasCalled(w => w.WriteFile("image/jpeg", "filename.jpg", "displayname"));
        }
    }
}