using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class OwinSettingsTester
    {
        [Test]
        public void default_static_file_rules()
        {
            new OwinSettings().StaticFileRules
                .Select(x => x.GetType()).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(DenyConfigRule));
        }

        [Test]
        public void if_we_build_an_app_from_scratch_will_have_the_asset_settings_tied_in()
        {
            using (var runtime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap())
            {
                runtime.Factory.Get<OwinSettings>().StaticFileRules.OfType<AssetSettings>()
                    .ShouldHaveCount(1);
            }
        }

        private AuthorizationRight forFile(string filename)
        {
            var file = new FubuFile(filename, null);
            var owinSettings = new OwinSettings();
            owinSettings.StaticFileRules.Add(new AssetSettings());

            return owinSettings.DetermineStaticFileRights(file);
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