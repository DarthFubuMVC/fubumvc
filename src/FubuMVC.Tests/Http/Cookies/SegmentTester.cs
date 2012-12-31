using System.Diagnostics;
using System.Web;
using FubuMVC.Core.Http.Cookies;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using FubuCore;

namespace FubuMVC.Tests.Http.Cookies
{
    [TestFixture]
    public class SegmentTester
    {
        [Test]
        public void parse_with_unquoted_value()
        {
            var segment = new Segment("a=1");

            segment.Key.ShouldEqual("a");
            segment.Value.ShouldEqual("1");
        }

        [Test]
        public void parse_with_no_value_uses_defaults()
        {
            var segment = new Segment("a");
            segment.Key.ShouldEqual("a");
            segment.Value.ShouldBeNull();
        }

        [Test]
        public void parse_with_quoted_value()
        {
            var segment = new Segment("a=\"how are you doing\"");

            segment.Key.ShouldEqual("a");
            segment.Value.ShouldEqual("how are you doing");
        }

        [Test]
        public void only_splits_on_first_equals()
        {
            var segment = new Segment("TestCookie=a1=b1&a2=b2");
            segment.Key.ShouldEqual("TestCookie");

            segment.Value.ShouldEqual("a1=b1&a2=b2");
        }
    }
}