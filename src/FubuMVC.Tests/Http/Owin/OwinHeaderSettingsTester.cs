using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinHeaderSettingsTester
    {
        private OwinHeaderSettings theSettings;

        [SetUp]
        public void SetUp()
        {
            theSettings = new OwinHeaderSettings();
        }

        [Test]
        public void allow_multiple_defaults_to_true()
        {
            theSettings.AllowMultiple(HttpRequestHeaders.Authorization).ShouldBeTrue();
        }

        [Test]
        public void allow_multiple_returns_false_if_header_has_been_excluded()
        {
            theSettings.DoNotAllowMultipleValues(HttpRequestHeaders.ContentType);
            theSettings.AllowMultiple(HttpRequestHeaders.ContentType).ShouldBeFalse();
        }

        [Test] // Keeps NOWIN happy
        public void allow_multiple_defaults_to_false_for_content_length()
        {
            theSettings.AllowMultiple(HttpRequestHeaders.ContentLength).ShouldBeFalse();
        }
    }
}
