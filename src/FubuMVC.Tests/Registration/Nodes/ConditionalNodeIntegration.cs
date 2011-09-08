using System;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes.ConditionalTest
{
    [TestFixture]
    public class ConditionalNodeIntegration
    {
        private FubuRegistry _fubuRegistry;
        private BehaviorGraph _graph;

        [SetUp]
        public void setup_chain()
        {
            _fubuRegistry = new FubuRegistry();

            _fubuRegistry.Actions
                .IncludeTypes(t => t.Namespace == typeof(ConditionalNodeIntegration).Namespace)
                .IncludeClassesSuffixedWithController();

            _fubuRegistry.Applies
                .ToThisAssembly();

            _fubuRegistry.Policies.Add<ConditionalTestPolicy>();

            _graph = _fubuRegistry.BuildGraph();
        }

        private ConditionalNode getDerpNode()
        {
            return _graph
                .BehaviorFor<TestConditionalController>(x => x.Derp())
                .OfType<ConditionalNode>().First();
        }

        private ConditionalNode getHerpNode()
        {
            return _graph
                .BehaviorFor<TestConditionalController>(x => x.Herp())
                .OfType<ConditionalNode>().First();
        }

        private ConditionalNode<CurrentRequest> getHerpDerpNode()
        {
            return _graph
                .BehaviorFor<TestConditionalController>(x => x.HerpDerp())
                .OfType<ConditionalNode<CurrentRequest>>().First();
        }

        [Test]
        public void the_condition_should_be_true()
        {
            getDerpNode().Condition().ShouldBeTrue();
        }

        [Test]
        public void inner_behavior_node_should_be_the_wrapper_node()
        {
            getDerpNode().InnerNode.ShouldBeOfType<Wrapper>();
        }

        [Test]
        public void inner_behavior_node_should_be_the_derp_behavior()
        {
            getDerpNode()
                .InnerNode.As<Wrapper>()
                .BehaviorType.ShouldEqual(typeof(AddTheHerpBehavior));
        }

        [Test]
        public void should_contain_the_inner_node()
        {
            getHerpNode()
                .InnerNode.ShouldBeOfType<Wrapper>();
        }

        [Test]
        public void behavior_type_should_be_is_ajax()
        {
            getHerpNode()
                .BehaviorType.ShouldEqual(typeof (IsAjaxRequest));
        }
        [Test]
        public void condition_should_be_null()
        {
            getHerpNode()
                .Condition.ShouldBeNull();
        }

        [Test]
        public void condition_should_not_be_null()
        {
            getHerpDerpNode()
                .Condition.ShouldNotBeNull();
        }

        [Test]
        public void inner_node_should_not_be_null()
        {
            getHerpDerpNode()
                .InnerNode.ShouldBeOfType<Wrapper>();
        }
    }

    public class ConditionalTestPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph
                .BehaviorFor<TestConditionalController>(x => x.Derp())
                .AddToEnd(Wrapper.For<AddTheHerpBehavior>().ConditionallyRunIf(() => true));

            graph
             .BehaviorFor<TestConditionalController>(x => x.Herp())
             .AddToEnd(Wrapper.For<AddTheHerpBehavior>().ConditionallyRunByBehavior<IsAjaxRequest>());

            graph
                .BehaviorFor<TestConditionalController>(x => x.HerpDerp())
                .AddToEnd(Wrapper.For<AddTheHerpBehavior>()
                              .ConditionallyRunIf<CurrentRequest>(x => x.UserAgent.Contains("mobile")));
        }
    }

    public class TestConditionalController
    {
        public Derp Derp()
        {
            return new Derp(){Message = "Derp"};
        }

        public Derp Herp()
        {
            return new Derp(){Message = "Herp"};
        }

        public Derp HerpDerp()
        {
            return new Derp() { Message = "HerpDerp" };
        }
    }

    public class Derp
    {
        public string Message { get; set; }
    }

    public class AddTheHerpBehavior : BasicBehavior
    {
        private readonly IFubuRequest _fubuRequest;

        public AddTheHerpBehavior(IFubuRequest fubuRequest)
            : base(PartialBehavior.Executes)
        {
            _fubuRequest = fubuRequest;
        }
        protected override DoNext performInvoke()
        {
            var derp = _fubuRequest.Get<Derp>();
            derp.Message += "Herp";
            return DoNext.Continue;
        }
    }
}