using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;
using System.Linq;

namespace FubuMVC.Tests.Registration
{
    
    public class when_using_the_default_authorization_policy
    {
        private BehaviorGraph graph;
        private BehaviorChain goChain;
        private BehaviorChain moveChain;


        public when_using_the_default_authorization_policy()
        {
            var registry = new FubuRegistry(x => {
                x.Actions.IncludeClassesSuffixedWithController();

                x.Configure(g =>
                {
                    g.ChainFor<AuthorizedController>(c => c.Go(null)).Authorization.AddRole("RoleA");
                });
            });

            graph = BehaviorGraph.BuildFrom(registry);

            goChain = graph.ChainFor<AuthorizedController>(x => x.Go(null));
            moveChain = graph.ChainFor<AuthorizedController>(x => x.Move(null));
        }


        [Fact]
        public void do_not_attach_the_authorization_node_if_there_are_no_authorization_rules_for_a_chain()
        {
            moveChain.Top.Any(x => x is AuthorizationNode).ShouldBeFalse();
        }

        [Fact]
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