using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.OwinHost.Middleware.StaticFiles;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing.Middleware.StaticFiles
{
    [TestFixture]
    public class AssetStaticFileRuleTester
    {
        private readonly IStaticFileRule theRule = new AssetStaticFileRule();

        [Test]
        public void can_write_javascript_files()
        {
            theRule.IsAllowed(new FubuFile("foo.js", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("foo.coffee", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void can_write_css()
        {
            theRule.IsAllowed(new FubuFile("bar.css", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void can_write_htm_or_html()
        {
            theRule.IsAllowed(new FubuFile("bar.htm", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.html", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void can_write_images()
        {
            theRule.IsAllowed(new FubuFile("bar.jpg", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.gif", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.tif", null)).ShouldEqual(AuthorizationRight.Allow);
            theRule.IsAllowed(new FubuFile("bar.png", null)).ShouldEqual(AuthorizationRight.Allow);
        }

        [Test]
        public void none_if_the_mime_type_is_not_recognized()
        {
            theRule.IsAllowed(new FubuFile("bar.nonexistent", null)).ShouldEqual(AuthorizationRight.None);
        }

        [Test]
        public void none_if_not_an_asset_file_or_html()
        {
            theRule.IsAllowed(new FubuFile("bar.txt", null)).ShouldEqual(AuthorizationRight.None);
        }
        
    }
}