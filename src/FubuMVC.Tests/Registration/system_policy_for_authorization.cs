using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class when_using_the_default_authorization_policy
    {
        private BehaviorGraph graph;
        private BehaviorChain goChain;
        private BehaviorChain moveChain;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x =>
            {
                x.Actions.IncludeTypesNamed(t => t.EndsWith("Controller"));

                x.Configure(g =>
                {
                    g.BehaviorFor<AuthorizedController>(c => c.Go()).Authorization.AddRole("RoleA");
                });
            });

            graph = registry.BuildGraph();

            goChain = graph.BehaviorFor<AuthorizedController>(x => x.Go());
            moveChain = graph.BehaviorFor<AuthorizedController>(x => x.Move());
        }


        [Test]
        public void do_not_attach_the_authorization_node_if_there_are_no_authorization_rules_for_a_chain()
        {
            moveChain.Any(x => x is AuthorizationNode).ShouldBeFalse();
        }

        [Test]
        public void do_attach_the_authorization_node_if_there_is_an_explicit_authorization_rule_for_a_chain()
        {
            goChain.Top.ShouldBeTheSameAs(goChain.Authorization);
        }
    }


    public class AuthorizedController
    {
        public void Go()
        {
            
        }

        public void Move()
        {
            
        }
    }
}