using FubuMVC.Core.Http;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Http
{
    
    public class CurrentMimeTypeModelBinderTester
    {
        [Fact]
        public void matches_current_mime_type_only()
        {
            var binder = new CurrentMimeTypeModelBinder();
            binder.Matches(GetType()).ShouldBeFalse();
            binder.Matches(typeof(CurrentMimeType)).ShouldBeTrue();
        }

        // Actual bind needs to be done w/ e2e test
    }
}