using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.SessionState;
using FubuMVC.Tests.Http.Cookies;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.SessionState
{
    
    public class retrieving_an_existing_item_from_flash : InteractionContext<CookieFlashProvider>
    {
        private string theCookieValue;
        private RecordingOutputWriter theOutputWriter;
        private FlashTarget theTarget;

        protected override void beforeEach()
        {
            theCookieValue = CookieFlashProvider.ToBase64String("{\"Name\":\"Joel\"}");

            theOutputWriter = new RecordingOutputWriter();

            Services.Inject<IOutputWriter>(theOutputWriter);

            MockFor<ICookies>().Stub(x => x.Has(CookieFlashProvider.FlashKey)).Return(true);
            MockFor<ICookies>().Stub(x => x.GetValue(CookieFlashProvider.FlashKey)).Return(theCookieValue);

            theTarget = ClassUnderTest.Retrieve<FlashTarget>();
        }

        private Cookie theCookie { get { return theOutputWriter.LastCookie; } }

        [Fact]
        public void parses_the_json()
        {
            theTarget.ShouldBe(new FlashTarget {Name = "Joel"});
        }

        [Fact]
        public void removes_the_cookie()
        {
            theCookie.Matches(CookieFlashProvider.FlashKey).ShouldBeTrue();
            theCookie.Expires.Value.Year.ShouldBeLessThan(LocalSystemTime.Year);
        }
    }
}