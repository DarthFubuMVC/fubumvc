using System;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.SessionState;
using FubuMVC.Tests.Http.Cookies;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Tests.SessionState
{
    [TestFixture]
    public class flashing_an_object : InteractionContext<CookieFlashProvider>
    {
        private RecordingOutputWriter theOutputWriter;
        private string theJson;
        private object theFlashObject;

        protected override void beforeEach()
        {
            theOutputWriter = new RecordingOutputWriter();

            theFlashObject = Guid.NewGuid();
            theJson = "test test test";

            Services.Inject<IOutputWriter>(theOutputWriter);
            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Stub(x => x.ToJson(theFlashObject)).Return(theJson);

            ClassUnderTest.Flash(theFlashObject);
        }

        private Cookie theCookie { get { return theOutputWriter.LastCookie; } }

        [Test]
        public void writes_the_cookie()
        {
            theCookie.Matches(CookieFlashProvider.FlashKey).ShouldBeTrue();
            theCookie.Value.ShouldBe(theJson);
        }
    }

    public class FlashTarget
    {
        public string Name { get; set; }

        protected bool Equals(FlashTarget other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FlashTarget) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}