using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Endpoints;
using FubuMVC.Core.Security.Authorization;
using HtmlTags;
using NUnit.Framework;
using Shouldly;

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
        public void do_not_exclude_diagnostics_chains_by_default()
        {
            var chain = DiagnosticChain.For<AboutFubuDiagnostics>(_ => _.get_about());

            new AuthenticationSettings().ShouldBeExcluded(chain)
                .ShouldBeFalse();

        }

        [Test]
        public void exclude_diagnostics_when_explicitly_chosen()
        {
            var chain = DiagnosticChain.For<AboutFubuDiagnostics>(_ => _.get_about());

            new AuthenticationSettings
            {
                ExcludeDiagnostics = true
            }.ShouldBeExcluded(chain)
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
            settings.ShouldBeExcluded(new RoutedChain("foo")).ShouldBeFalse();
        }

        [Test]
        public void automatically_excludes_the_NotAuthenticated_attribute()
        {
            var chain = new RoutedChain("foo");
            chain.AddToEnd(ActionCall.For<AuthenticatedEndpoints>(x => x.get_notauthenticated()));

            new AuthenticationSettings().ShouldBeExcluded(chain)
                .ShouldBeTrue();
        }

        [Test]
        public void apply_a_custom_exclusion()
        {
            var chain = new RoutedChain("foo");
            chain.AddToEnd(ActionCall.For<AuthenticatedEndpoints>(x => x.get_tag()));


            var settings = new AuthenticationSettings();

            settings.ShouldBeExcluded(chain).ShouldBeFalse();

            settings.ExcludeChains = c => typeof (HtmlTag) == c.ResourceType();

            settings.ShouldBeExcluded(chain).ShouldBeTrue();
        }

        [Test]
        public void apply_a_custome_exclusion_and_it_does_not_apply_to_login_page()
        {
            var settings = new AuthenticationSettings();
            var chain = new RoutedChain("foo");
            chain.AddToEnd(ActionCall.For<LoginController>(x => x.get_login(null)));
            settings.ShouldBeExcluded(chain).ShouldBeTrue();

            settings.ExcludeChains = c => c.Calls.Count() == 5; // just need a fake

            settings.ShouldBeExcluded(chain).ShouldBeTrue();
        }

        [Test]
        public void exclude_by_default_if_the_input_type_is_marked_as_NotAuthenticated()
        {
            var chain = new RoutedChain("bar");
            chain.AddToEnd(ActionCall.For<AuthenticatedEndpoints>(x => x.post_something(null)));


            var settings = new AuthenticationSettings();

            settings.ShouldBeExcluded(chain).ShouldBeTrue();
        }

        [Test]
        public void exclude_by_default_actions_marked_as_pass_through()
        {
            var chain = new RoutedChain("foo");
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