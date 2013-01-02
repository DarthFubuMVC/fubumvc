using FubuTestingSupport;
using NUnit.Framework;
using FubuMVC.Core;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class UrlStringExtensionsTester
    {
        [Test]
        public void to_absolute_url_when_it_is_already_an_absolute_url()
        {
            "http://cnn.com".ToAbsoluteUrl("http://localhost:5050")
                        .ShouldEqual("http://cnn.com");
        }

        [Test]
        public void to_absolute_url_from_relative_to_absolute()
        {
            "/foo".ToAbsoluteUrl("http://localhost:5050")
                  .ShouldEqual("http://localhost:5050/foo");
        }


        [Test]
        public void to_absolute_url_from_relative_to_absolute_2()
        {
            "~/foo".ToAbsoluteUrl("http://localhost:5050")
                  .ShouldEqual("http://localhost:5050/foo");
        }
    }
}