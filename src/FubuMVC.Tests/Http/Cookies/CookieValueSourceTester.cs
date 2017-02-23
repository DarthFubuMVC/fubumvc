using System;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Http.Cookies
{
    
    public class CookieValueSourceTester : InteractionContext<CookieValueSource>
    {
        private ICookies inner;

        protected override void beforeEach()
        {
            inner = MockFor<ICookies>();
        }

        [Fact]
        public void provenance_has_to_be_cookies()
        {
            ClassUnderTest.Provenance.ShouldBe(RequestDataSource.Cookie.ToString());
        }

        [Fact]
        public void has_delegates()
        {
            ClassUnderTest.Has("a").ShouldBeFalse();


            inner.Stub(x => x.Has("b")).Return(true);

            ClassUnderTest.Has("b").ShouldBeFalse();
        }

        [Fact]
        public void get_with_single_value()
        {
            var cookie = new Cookie("a", "1");
            cookie.Value.ShouldBe("1");

            inner.Stub(x => x.Get("a")).Return(cookie);

            ClassUnderTest.Get("a").ShouldBe("1");
        }

        [Fact]
        public void value_with_miss()
        {
            ClassUnderTest.Value("a", v => {
                throw new Exception("should not be called");
            }).ShouldBeFalse();
        }

        [Fact]
        public void value_with_single_value_hit()
        {
            var cookie = new Cookie("a", "1");
            inner.Stub(x => x.Get("a")).Return(cookie);

            bool wasCalled = false;
            ClassUnderTest.Value("a", v => {
                wasCalled = true;

                v.RawKey.ShouldBe("a");
                v.RawValue.ShouldBe("1");
                v.Source.ShouldBe(ClassUnderTest.Provenance);
            }).ShouldBeTrue();

            wasCalled.ShouldBeTrue();
        }

        [Fact]
        public void value_that_hits_but_not_deterministic()
        {
            var cookie = new Cookie("a");
            cookie.Value.ShouldBeNull();
            inner.Stub(x => x.Get("a")).Return(cookie);

            ClassUnderTest.Value("a", v =>
            {
                throw new Exception("should not be called");
            }).ShouldBeFalse();
        }


    }
}