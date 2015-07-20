using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin.Middleware.StaticFiles
{
    [TestFixture]
    public class DenyConfigRuleTester
    {
        [Test]
        public void deny_config_files_period()
        {
            var theRule = new DenyConfigRule();

            theRule.IsAllowed(new FubuFile("foo.config")).ShouldEqual(AuthorizationRight.Deny);
            theRule.IsAllowed(new FubuFile("web.config")).ShouldEqual(AuthorizationRight.Deny);
            theRule.IsAllowed(new FubuFile("foo.asset.config")).ShouldEqual(AuthorizationRight.Deny);
        }

        [Test]
        public void no_opinion_about_anything_else()
        {
            var theRule = new DenyConfigRule();

            theRule.IsAllowed(new FubuFile("foo.txt")).ShouldEqual(AuthorizationRight.None);
            theRule.IsAllowed(new FubuFile("foo.htm")).ShouldEqual(AuthorizationRight.None);
            theRule.IsAllowed(new FubuFile("foo.jpg")).ShouldEqual(AuthorizationRight.None);
        }
    }
}