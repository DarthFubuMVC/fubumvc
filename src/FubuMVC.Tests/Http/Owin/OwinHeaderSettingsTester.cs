using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.Owin
{
    
    public class OwinHeaderSettingsTester
    {
        private OwinHeaderSettings theSettings = new OwinHeaderSettings();

        [Fact]
        public void allow_multiple_defaults_to_true()
        {
            theSettings.AllowMultiple(HttpRequestHeaders.Authorization).ShouldBeTrue();
        }

        [Fact]
        public void allow_multiple_returns_false_if_header_has_been_excluded()
        {
            theSettings.DoNotAllowMultipleValues(HttpRequestHeaders.ContentType);
            theSettings.AllowMultiple(HttpRequestHeaders.ContentType).ShouldBeFalse();
        }

        [Fact] // Keeps NOWIN happy
        public void allow_multiple_defaults_to_false_for_content_length()
        {
            theSettings.AllowMultiple(HttpRequestHeaders.ContentLength).ShouldBeFalse();
        }
    }
}
