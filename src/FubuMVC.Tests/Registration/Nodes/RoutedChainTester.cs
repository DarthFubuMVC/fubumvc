using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class RoutedChainTester
    {
        [Test]
        public void adds_the_authentication_node_if_it_exists()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<AuthenticatedEndpoint>();
            registry.Configure(
                graph => { graph.Chains.OfType<RoutedChain>().Each(x => x.Authentication = new AuthNode()); });

            using (var runtime = registry.ToRuntime())
            {
                runtime.Behaviors.ChainFor<AuthenticatedEndpoint>(x => x.get_hello())
                    .First().ShouldBeOfType<AuthNode>();
            }


            var chain = new RoutedChain("something");
            var auth = new AuthNode();
            chain.Authentication = auth;
        }
    }

    public class AuthenticatedEndpoint
    {
        public string get_hello()
        {
            return "hello";
        }
    }

    public class FakeAuthBehavior : IActionBehavior
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }

    public class AuthNode : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Authentication; }
        }

        protected override IConfiguredInstance buildInstance()
        {
            return new SmartInstance<FakeAuthBehavior>();
        }
    }
}