using FubuCore;
using FubuMVC.Diagnostics.Configuration.Policies;
using FubuMVC.Tests;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Configuration.Policies
{
    [TestFixture]
    public class DiagnosticsUrlsTester
    {
        [Test]
        public void should_return_path_if_no_tilde_is_found()
        {
            var somePath = "some-path";
            DiagnosticsUrls
                .ToRelativeUrl(somePath).ShouldEqual(somePath);
        }

        [Test]
        public void should_return_path_if_invalid_prefix_is_specified()
        {
            var invalidPrefixedPath = "~some-path";
            DiagnosticsUrls
                .ToRelativeUrl(invalidPrefixedPath).ShouldEqual(invalidPrefixedPath);
        }

        [Test]
        public void should_return_relative_path_when_prefix_is_found()
        {
            DiagnosticsUrls
                .ToRelativeUrl("~/some-path")
                .ShouldEqual("{0}/some-path".ToFormat(DiagnosticsUrls.ROOT));
        }

        [Test]
        public void should_return_null_if_path_is_null_or_empty()
        {
            DiagnosticsUrls
                .ToRelativeUrl(null)
                .ShouldBeNull();
        }

        [Test]
        public void should_not_have_trailing_slash_for_root_route()
        {
            DiagnosticsUrls
                .ToRelativeUrl("~/")
                .EndsWith("/")
                .ShouldBeFalse();
        }
    }
}