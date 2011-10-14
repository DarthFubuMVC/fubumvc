using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class CurrentMimeTypeTester
    {
        [Test]
        public void feeding_in_null_for_contentType_defaults_to_HttpFormMimeType()
        {
            var currentMimeType = new CurrentMimeType(null, null);
            currentMimeType.ContentType.ShouldEqual(MimeType.HttpFormMimetype);
        }

        [Test]
        public void is_smart_enough_to_pull_out_charset()
        {
            var currentMimeType = new CurrentMimeType("application/x-www-form-urlencoded; charset=UTF-8", null);
            currentMimeType.ContentType.ShouldEqual("application/x-www-form-urlencoded");
            currentMimeType.Charset.ShouldEqual("UTF-8");
        }
    }
}