using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class AttachOutputPolicyTester
    {
        private AttachOutputPolicy _outputPolicy;
        private BehaviorGraph _graph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _outputPolicy = new AttachOutputPolicy();
            _graph = new BehaviorGraph();
            _graph.AddChain(BehaviorChain.For<TestEndpoint>(e => e.X()));
            _graph.AddChain(BehaviorChain.For<TestEndpoint>(e => e.AnyNumber()));
            _outputPolicy.Configure(_graph);
        }

        [Test]
        public void should_not_add_nodes_with_continuation_output()
        {
            _graph.Behaviors.ShouldNotHave(bc => bc.Outputs.Select(node => node.BehaviorType).Any(t => t.CanBeCastTo<FubuContinuation>()));
        }

        [Test]
        public void should_not_add_nodes_with_no_output()
        {
            _graph.Behaviors.ShouldNotHave(bc => bc.Outputs.Select(node => node.BehaviorType).Any(t => t == typeof(void)));
        }
    }

    public class TestEndpoint
    {
        public FubuContinuation X()
        {
            return FubuContinuation.EndWithStatusCode(HttpStatusCode.Unused);
        }
        public void AnyNumber()
        {
            
        }
    }
}
