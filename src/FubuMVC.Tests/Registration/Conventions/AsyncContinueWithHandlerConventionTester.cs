using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class AsyncContinueWithHandlerConventionTester
    {
        private BehaviorGraph graph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x => x.Actions.IncludeType<TestControllerForAsync>());
        }

        [Test]
        public void should_attach_async_node_to_actions_that_return_a_task_with_result()
        {
            graph.BehaviorFor<TestControllerForAsync>(x => x.ActionWithInputWithOutputAsync(null))
                .Any(x => x is AsyncContinueWithNode)
                .ShouldBeTrue();
        }

        [Test]
        public void should_attach_async_node_to_actions_that_return_a_task_with_no_result()
        {
            graph.BehaviorFor<TestControllerForAsync>(x => x.ActionWithInputNoOutputAsync(null))
                .Any(x => x is AsyncContinueWithNode)
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_attach_async_node_to_actions_that_return_no_task()
        {
            graph.BehaviorFor<TestControllerForAsync>(x => x.NotAsync()).Top
                .Any(x => x is AsyncContinueWithNode)
                .ShouldBeFalse();
        }

        [Test]
        public void the_actions_that_return_task_with_continuation_should_have_async_node_then_continuation_node()
        {
            graph.BehaviorFor<TestControllerForAsync>(x => x.ActionWithContinuationAsync())
                .FirstCall().Next.ShouldBeOfType<AsyncContinueWithNode>()
                .Next.ShouldBeOfType<ContinuationNode>();
        }


        public class Input {}
        public class Output {}

        public class TestControllerForAsync
        {
            public Task ActionWithInputNoOutputAsync(Input input)
            {
                return Task.Factory.StartNew(() => { });
            }

            public Task<Output> ActionWithInputWithOutputAsync(Input input)
            {
                return Task<Output>.Factory.StartNew(() => new Output());
            }

            public Task<FubuContinuation> ActionWithContinuationAsync()
            {
                return Task<FubuContinuation>.Factory.StartNew(() => FubuContinuation.RedirectTo<Input>());
            }

            public Output NotAsync()
            {
                return null;
            }
        }
    }
}