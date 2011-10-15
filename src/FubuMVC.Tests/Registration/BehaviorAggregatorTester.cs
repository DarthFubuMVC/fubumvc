using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class BehaviorAggregatorTester
    {
        private BehaviorAggregator _aggregator;

        [SetUp]
        public void setup()
        {
            _aggregator = new BehaviorAggregator(new TypePool(null), new List<IActionSource>
                                                                         {
                                                                             new ActionSourceStub(() => new List<ActionCall>
                                                                                                            {
                                                                                                                ActionCall.For<TestController>(c => c.Index())
                                                                                                            }),
                                                                            new ActionSourceStub(() => new List<ActionCall>
                                                                                                            {
                                                                                                                ActionCall.For<TestController>(c => c.SomeAction(1))
                                                                                                            })
                                                                         });
        }

        [Test]
        public void should_register_behavior_chains_for_action_calls()
        {
            var graph = new BehaviorGraph();
            _aggregator.Configure(graph);

            graph
                .Behaviors
                .ShouldHaveCount(2);

            graph
                .Behaviors
                .ShouldContain(chain =>
                                   {
                                       var call = chain.FirstCall();
                                       return call.HandlerType.Equals(typeof (TestController))
                                              && call.Method.Name.Equals("Index");
                                   });

            graph
                .Behaviors
                .ShouldContain(chain =>
                                   {
                                       var call = chain.FirstCall();
                                       return call.HandlerType.Equals(typeof (TestController))
                                              && call.Method.Name.Equals("SomeAction");
                                   });

        }

        public class ActionSourceStub : IActionSource
        {
            private readonly Func<IEnumerable<ActionCall>> _builder;

            public ActionSourceStub(Func<IEnumerable<ActionCall>> builder)
            {
                _builder = builder;
            }

            public IEnumerable<ActionCall> FindActions(TypePool types)
            {
                return _builder();
            }
        }
    }
}