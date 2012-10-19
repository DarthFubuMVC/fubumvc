using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class BehaviorAggregatorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void setup()
        {
            theActions = new ActionSources();
            theActions.AddSource(new ActionSourceStub(() => new List<ActionCall>
            {
                ActionCall.For<TestController>(c => c.Index())
            }));

            theActions.AddSource(new ActionSourceStub(() => new List<ActionCall>
            {
                ActionCall.For<TestController>(c => c.SomeAction(1))
            }));

            _aggregator = new BehaviorAggregator();
        }

        #endregion

        private BehaviorAggregator _aggregator;
        private ActionSources theActions;

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

        [Test]
        public void should_register_behavior_chains_for_action_calls()
        {
            var graph = new BehaviorGraph();
            graph.Settings.Replace(theActions);

            _aggregator.Configure(graph);

            graph
                .Behaviors
                .ShouldHaveCount(2);

            graph
                .Behaviors
                .ShouldContain(chain => {
                    var call = chain.FirstCall();
                    return call.HandlerType.Equals(typeof (TestController))
                           && call.Method.Name.Equals("Index");
                });

            graph
                .Behaviors
                .ShouldContain(chain => {
                    var call = chain.FirstCall();
                    return call.HandlerType.Equals(typeof (TestController))
                           && call.Method.Name.Equals("SomeAction");
                });
        }
    }
}