using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Ajax
{
    [TestFixture]
    public class AjaxContinuationPolicyIntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<Controller1>();

            theGraph = registry.BuildGraph();
        }

        #endregion

        private BehaviorGraph theGraph;

        private BehaviorChain chainFor(Expression<Action<Controller1>> method)
        {
            return theGraph.BehaviorFor(method);
        }


        public class Input
        {
        }

        public class Output
        {
        }

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

        [Test]
        public void no_behavior_on_actions_that_do_not_return_continuations()
        {
            chainFor(x => x.NoContinuation(null))
                .Output.OfType<Writer>().Any().ShouldBeFalse();
        }

        [Test]
        public void should_be_a_behavior_on_actions_that_return_a_subclass_of_AjaxContinuation()
        {
            chainFor(x => x.SpecialContinuation(null))
                .Output.Writers.OfType<Writer>()
                .Single()
                .WriterType.ShouldEqual(typeof(AjaxContinuationWriter<MySpecialContinuation>));
        }

        [Test]
        public void should_be_a_behavior_on_actions_that_return_the_AjaxContinuation()
        {
            chainFor(x => x.BasicContinuation(null)).Output.Writers.OfType<Writer>().Single()
                .WriterType.ShouldEqual(typeof (AjaxContinuationWriter<AjaxContinuation>));
        }

        [Test]
        public void should_have_a_conneg_input_node_with_json_or_http_post_input()
        {
            var connegInput = chainFor(x => x.BasicContinuation(null)).Input;
            connegInput.AllowHttpFormPosts.ShouldBeTrue();
            connegInput.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void should_only_apply_behavior_once()
        {
            var hostRegistry = new FubuRegistry();
            var packageRegistry = new FubuPackageRegistry();
            packageRegistry.Actions.IncludeType<Controller1>();
            hostRegistry.Import(packageRegistry, string.Empty);
            theGraph = hostRegistry.BuildGraph();

            var chain = chainFor(x => x.BasicContinuation(null))
                .Output.Writers.OfType<Writer>().ShouldHaveCount(1);
        }
    }
}