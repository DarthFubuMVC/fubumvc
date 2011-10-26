using System;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Ajax
{
    [TestFixture]
    public class AjaxContinuationPolicyIntegratedTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            theGraph = registry.BuildGraph();
        }

        private BehaviorChain chainFor(Expression<Action<Controller1>> method)
        {
            return theGraph.BehaviorFor(method);
        }

        [Test]
        public void no_behavior_on_actions_that_do_not_return_continuations()
        {
            chainFor(x => x.NoContinuation(null))
                .Any(x => x is AjaxContinuationNode)
                .ShouldBeFalse();
        }

        [Test]
        public void should_be_a_behavior_on_actions_that_return_the_AjaxContinuation()
        {
            chainFor(x => x.BasicContinuation(null))
                .Any(x => x is AjaxContinuationNode)
                .ShouldBeTrue();
        }

        [Test]
        public void should_be_a_behavior_on_actions_that_return_a_subclass_of_AjaxContinuation()
        {
            chainFor(x => x.SpecialContinuation(null))
                .Any(x => x is AjaxContinuationNode)
                .ShouldBeTrue();
        }

        [Test]
        public void should_only_apply_behavior_once()
        {
            var hostRegistry = new FubuRegistry();
            var packageRegistry = new FubuPackageRegistry();
            packageRegistry.Actions.IncludeType<Controller1>();
            hostRegistry.Import(packageRegistry, string.Empty);
            theGraph = hostRegistry.BuildGraph();

            chainFor(x => x.BasicContinuation(null))
                .Where(x => x is AjaxContinuationNode)
                .ShouldHaveCount(1);
        }



        public class Input{}
        public class Output{}
        public class Controller1
        {
            public Output NoContinuation(Input input)
            {
                return null;
            }

            public AjaxContinuation BasicContinuation(Input input)
            {
                return null;
            }

            public MySpecialContinuation SpecialContinuation(Input input)
            {
                return null;
            }
        }

        public class MySpecialContinuation : AjaxContinuation
        {
            
        }
    }
}