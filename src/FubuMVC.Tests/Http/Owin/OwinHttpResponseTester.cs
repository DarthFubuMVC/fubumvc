using System.Linq;
using FubuMVC.Core.Http.Owin;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinHttpResponseTester
    {
        [Test]
        public void do_not_blow_up_when_there_are_no_headers()
        {
            new OwinHttpResponse().AllHeaders().Any().ShouldBeFalse();
        }
    }
}