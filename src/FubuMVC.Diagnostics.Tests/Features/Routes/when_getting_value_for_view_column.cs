using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Diagnostics.Core.Grids.Columns.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Features.Routes
{
    [TestFixture]
    public class when_getting_value_for_view_column : InteractionContext<ViewColumn>
    {
        private BehaviorGraph _graph;
		
		protected override void beforeEach()
		{
            _graph = new FubuRegistry(registry =>
             {
                 registry.Applies.ToThisAssembly();
                 registry.Actions.IncludeType<Test>();
                 registry.Actions.IncludeMethods(method => (new[] { "Index", "Continuation", "ZeroModelOut", "HasOutputNode" }).Contains(method.Name));
                 registry.ApplyConvention<OutputNodeConvention>();
				
             }).BuildGraph();
        }

        [Test]
        public void na_if_zero_model_out()
        {
            ClassUnderTest.ValueFor(_graph.BehaviorFor<Test>(x => x.ZeroModelOut())).ShouldEqual(ViewColumn.NotApplicable);
        }

        [Test]
        public void na_if_empty_behavior_chain()
        {
            ClassUnderTest.ValueFor(new BehaviorChain()).ShouldEqual(ViewColumn.NotApplicable);
        }

        [Test]
        public void none_if_behavior_chain_has_no_output()
        {
            ClassUnderTest.ValueFor(_graph.BehaviorFor<Test>(x => x.Index())).ShouldEqual(ViewColumn.None);
        }

        [Test]
        public void value_if_actioncall_has_output_node()
        {
            ClassUnderTest.ValueFor(_graph.BehaviorFor<Test>(x => x.HasOutputNode())).ShouldEqual(new OutputNode(GetType()).ToString());
        }

        public class Test
        {
            public Test Index()
            {
                return this;
            }

            public FubuContinuation Continuation()
            {
                return FubuContinuation.NextBehavior();
            }

            public Test HasOutputNode()
            {
                return this;
            }

            public void ZeroModelOut()
            {
                
            }
        }

        public class OutputNodeConvention : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                graph.Actions()
                    .Where(call => call.Method.Name == "HasOutputNode")
                    .Each(call => call.ParentChain().ApplyConneg());
            }
        }
    }
}