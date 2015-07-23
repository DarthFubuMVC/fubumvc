using FubuMVC.Core.Http;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Http
{
    [TestFixture]
    public class CurrentMimeTypeModelBinderTester
    {
        [Test]
        public void matches_current_mime_type_only()
        {
            var binder = new CurrentMimeTypeModelBinder();
            binder.Matches(GetType()).ShouldBeFalse();
            binder.Matches(typeof(CurrentMimeType)).ShouldBeTrue();
        }

        // Actual bind needs to be done w/ e2e test
    }
}