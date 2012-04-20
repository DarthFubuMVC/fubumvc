using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.WebForms;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Tests.View.WebForms
{
    [TestFixture]
    public class when_using_the_WebFormsEndpoint_attribute
    {
        private BehaviorGraph _graph;
        private IConfigurationObserver _observer;

        [SetUp]
        public void SetUp()
        {
            _observer = MockRepository.GenerateStub<IConfigurationObserver>();
            _graph = new FubuRegistry(x =>
                    {
                        x.Actions.IncludeType<FakeController>();
                        x.UsingObserver(_observer);
                    }).BuildGraph();
        }

        public class FakeController
        {
            [WebFormsEndpoint(typeof(FakeView))]
            public void DoNothing(FakeInput input){}
        }
        public class FakeInput { }
        public class FakeView : FubuPage {}

        [Test]
        public void should_append_behavioral_node_to_void_end_point_attributed_calls()
        {
            Assert.Fail("NWO");
            //var token = new WebFormViewToken(typeof(FakeView));
            //var actions = _graph.Actions().Where(x => !x.HasAnyOutputBehavior()
            //    && x.Method.HasAttribute<WebFormsEndpointAttribute>()).ToList();
            //for (int index = 0; index < actions.Count; index++)
            //{
            //    var call = actions[index];
            //    call.LastOrDefault().ShouldBeTheSameAs(token.ToBehavioralNode());
            //}
        }

        [Test]
        public void observer_should_record_call_status()
        {
            var token = new WebFormViewToken(typeof(FakeView));
            var actions = _graph.Actions().Where(x => !x.HasAnyOutputBehavior()
                && x.Method.HasAttribute<WebFormsEndpointAttribute>()).ToList();
            for (int index = 0; index < actions.Count; index++)
            {
                var call = actions[index];
                _observer.AssertWasCalled(o => o.RecordCallStatus(Arg<ActionCall>.Is.Equal(call),
                    Arg<string>.Matches(s => s.Contains(
                        "Action '{0}' has {1} declared, using WebForms view '{2}'"
                        .ToFormat(call.Description, "WebFormsEndpointAttribute", token)))));
            }
        }
    }
}