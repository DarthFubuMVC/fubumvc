using System.Linq;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.OwinHost.Middleware.StaticFiles;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.OwinHost.Testing
{
    [TestFixture]
    public class OwinSettingsTester
    {
        [Test]
        public void default_static_file_rules()
        {
            new OwinSettings().StaticFileRules
                .Select(x => x.GetType()).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(AssetStaticFileRule), typeof(DenyConfigRule));
        }

        private AuthorizationRight forFile(string filename)
        {
            var file = new FubuFile(filename, null);
            return new OwinSettings().DetermineStaticFileRights(file);
        }

        [Test]
        public void assert_static_rights()
        {
            forFile("foo.txt").ShouldEqual(AuthorizationRight.None);
            forFile("foo.config").ShouldEqual(AuthorizationRight.Deny);
            forFile("foo.jpg").ShouldEqual(AuthorizationRight.Allow);
            forFile("foo.css").ShouldEqual(AuthorizationRight.Allow);
            forFile("foo.bmp").ShouldEqual(AuthorizationRight.Allow);
        }
    }
}