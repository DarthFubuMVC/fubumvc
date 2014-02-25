using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using FubuTestingSupport;
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
            var registry = new FubuRegistry(x => {
                x.Actions.IncludeClassesSuffixedWithController();

                x.Configure(g =>
                {
                    g.BehaviorFor<AuthorizedController>(c => c.Go(null)).Authorization.AddRole("RoleA");
                });
            });

            graph = BehaviorGraph.BuildFrom(registry);

            goChain = graph.BehaviorFor<AuthorizedController>(x => x.Go(null));
            moveChain = graph.BehaviorFor<AuthorizedController>(x => x.Move(null));
        }


        [Test]
        public void do_not_attach_the_authorization_node_if_there_are_no_authorization_rules_for_a_chain()
        {
            moveChain.Top.Any(x => x is AuthorizationNode).ShouldBeFalse();
        }

        [Test]
        public void do_attach_the_authorization_node_if_there_is_an_explicit_authorization_rule_for_a_chain()
        {
            goChain.ShouldContain(goChain.Authorization.As<AuthorizationNode>());
        }
    }


    public class AuthorizedController
    {
        public void Go(InputModel model)
        {
            
        }

        public void Move(InputModel model)
        {
            
        }
    }
}