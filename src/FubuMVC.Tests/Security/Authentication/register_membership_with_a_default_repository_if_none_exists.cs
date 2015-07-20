using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class register_membership_with_a_default_repository_if_none_exists
    {
        [Test]
        public void insert_membership_node_when_it_is_enabled_and_missing()
        {
            var graph = new BehaviorGraph();
            graph.Settings.Get<AuthenticationSettings>()
                .MembershipEnabled = MembershipStatus.Enabled;

            new RegisterAuthenticationStrategies().Configure(graph);

            graph.Settings.Get<AuthenticationSettings>()
                 .Strategies.Single().ShouldBeOfType<MembershipNode>();
        }

        [Test]
        public void do_not_insert_membership_node_if_there_is_already_one()
        {
            var graph = new BehaviorGraph();
            var settings = graph.Settings.Get<AuthenticationSettings>();
            settings.MembershipEnabled = MembershipStatus.Enabled;
            settings.Strategies.InsertFirst(new MembershipNode());

            new RegisterAuthenticationStrategies().Configure(graph);

            settings.Strategies.Count().ShouldEqual(1);
        }

        [Test]
        public void do_not_insert_memebership_node_if_membership_is_disabled()
        {
            var graph = new BehaviorGraph();
            var settings = graph.Settings.Get<AuthenticationSettings>();
            settings.MembershipEnabled = MembershipStatus.Disabled;

            new RegisterAuthenticationStrategies().Configure(graph);

            settings.Strategies.Any().ShouldBeFalse();
        }
    }
}