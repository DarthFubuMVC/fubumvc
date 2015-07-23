using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class register_membership_with_a_default_repository_if_none_exists
    {
        [Test]
        public void insert_membership_node_when_it_is_enabled_and_missing()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Features.Authentication.Configure(_ =>
                {
                    _.Enabled = true;
                    _.MembershipEnabled = MembershipStatus.Enabled;
                });
            });

            graph.Settings.Get<AuthenticationSettings>()
                 .Strategies.Single().ShouldBeOfType<MembershipNode>();
        }

        [Test]
        public void do_not_insert_membership_node_if_there_is_already_one()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Features.Authentication.Configure(_ =>
                {
                    _.Enabled = true;
                    _.MembershipEnabled = MembershipStatus.Enabled;
                    _.Strategies.InsertFirst(new MembershipNode());
                });
            });

            var settings = graph.Settings.Get<AuthenticationSettings>();
  
            settings.Strategies.Count().ShouldBe(1);
        }

        [Test]
        public void do_not_insert_memebership_node_if_membership_is_disabled()
        {

            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Features.Authentication.Configure(_ =>
                {
                    _.Enabled = true;
                    _.MembershipEnabled = MembershipStatus.Disabled;
                });
            });

            var settings = graph.Settings.Get<AuthenticationSettings>();

            settings.Strategies.Any().ShouldBeFalse();
        }
    }
}