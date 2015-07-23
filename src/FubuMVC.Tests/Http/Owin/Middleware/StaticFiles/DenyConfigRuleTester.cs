using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
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

            theRule.IsAllowed(new FubuFile("foo.config")).ShouldBe(AuthorizationRight.Deny);
            theRule.IsAllowed(new FubuFile("web.config")).ShouldBe(AuthorizationRight.Deny);
            theRule.IsAllowed(new FubuFile("foo.asset.config")).ShouldBe(AuthorizationRight.Deny);
        }

        [Test]
        public void no_opinion_about_anything_else()
        {
            var theRule = new DenyConfigRule();

            theRule.IsAllowed(new FubuFile("foo.txt")).ShouldBe(AuthorizationRight.None);
            theRule.IsAllowed(new FubuFile("foo.htm")).ShouldBe(AuthorizationRight.None);
            theRule.IsAllowed(new FubuFile("foo.jpg")).ShouldBe(AuthorizationRight.None);
        }
    }
}