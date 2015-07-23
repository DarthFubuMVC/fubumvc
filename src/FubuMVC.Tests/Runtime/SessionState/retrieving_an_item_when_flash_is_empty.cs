using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime.SessionState;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.SessionState
{
    [TestFixture]
    public class retrieving_an_item_when_flash_is_empty : InteractionContext<CookieFlashProvider>
    {
        protected override void beforeEach()
        {
            MockFor<ICookies>().Stub(x => x.Has(CookieFlashProvider.FlashKey)).Return(false);
        }

        [Test]
        public void returns_default_for_type()
        {
            ClassUnderTest.Retrieve<FlashTarget>().ShouldBeNull();
            ClassUnderTest.Retrieve<int>().ShouldBe(default(int));
        }
    }
}