using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.Security.Authorization;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class AuthenticationSettingsTester
    {
        [Test]
        public void disabled_by_default()
        {
            new AuthenticationSettings().Enabled.ShouldBeFalse();
        }

        [Test]
        public void has_to_be_application_level()
        {
            typeof(AuthenticationSettings).HasAttribute<ApplicationLevelAttribute>()
                .ShouldBeTrue();
        }

        [Test]
        public void membership_status_is_enabled_by_default()
        {
            new AuthenticationSettings().MembershipEnabled
                                        .ShouldBe(MembershipStatus.Enabled);
        }

        [Test]
        public void excludes_is_always_false_with_no_exclusions()
        {
            var settings = new AuthenticationSettings();
            settings.ShouldBeExcluded(new BehaviorChain()).ShouldBeFalse();
        }

        [Test]
        public void automatically_excludes_the_NotAuthenticated_attribute()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<AuthenticatedEndpoints>(x => x.get_notauthenticated()));

            new AuthenticationSettings().ShouldBeExcluded(chain)
                .ShouldBeTrue();
        }

        [Test]
        public void apply_a_custom_exclusion()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<AuthenticatedEndpoints>(x => x.get_tag()));


            var settings = new AuthenticationSettings();

            settings.ShouldBeExcluded(chain).ShouldBeFalse();

            settings.ExcludeChains.ResourceTypeIs<HtmlTag>();

            settings.ShouldBeExcluded(chain).ShouldBeTrue();

        }

        [Test]
        public void apply_a_custome_exclusion_and_it_does_not_apply_to_login_page()
        {
            var settings = new AuthenticationSettings();
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<LoginController>(x => x.get_login(null)));
            settings.ShouldBeExcluded(chain).ShouldBeTrue();
            
            settings.ExcludeChains.ChainMatches(c => c.Calls.Count() == 5); // just need a fake

            settings.ShouldBeExcluded(chain).ShouldBeTrue();
        }

        [Test]
        public void exclude_by_default_if_the_input_type_is_marked_as_NotAuthenticated()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<AuthenticatedEndpoints>(x => x.post_something(null)));


            var settings = new AuthenticationSettings();

            settings.ShouldBeExcluded(chain).ShouldBeTrue();
        }

		[Test]
		public void exclude_by_default_actions_marked_as_pass_through()
		{
			var chain = new BehaviorChain();
			chain.AddToEnd(ActionCall.For<AuthenticatedEndpoints>(x => x.get_pass_through_authentication()));


			var settings = new AuthenticationSettings();

			settings.ShouldBeExcluded(chain).ShouldBeTrue();
		}
    }

    public class AuthenticatedEndpoints
    {
        [NotAuthenticated]
        public string get_notauthenticated()
        {
            return "anything";
        }
        
        public string get_authenticated()
        {
            return "else";
        }

        public HtmlTag get_tag()
        {
            return new HtmlTag("div");
        }

		[PassThroughAuthentication]
		public string get_pass_through_authentication()
		{
			return "hello, everybody";
		}

        public void post_something(NotAuthenticatedMessage message)
        {
            
        }
    }

    [NotAuthenticated]
    public class NotAuthenticatedMessage
    {
        
    }
}