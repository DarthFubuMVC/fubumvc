using FubuMVC.Core.Http.Cookies;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Http.Cookies
{
    [TestFixture]
    public class CookieTester
    {
        

        [Test]
        public void matches()
        {
            var cookie = new Cookie();
            cookie.Matches("a").ShouldBeFalse();

            cookie.Add(new CookieState("something"));

            cookie.Matches("a").ShouldBeFalse();
            cookie.Matches("something").ShouldBeTrue();

            cookie.Add(new CookieState("Else"));
            cookie.Matches("something").ShouldBeTrue();
            cookie.Matches("else").ShouldBeTrue();
            cookie.Matches("a").ShouldBeFalse();
        }
    }
}