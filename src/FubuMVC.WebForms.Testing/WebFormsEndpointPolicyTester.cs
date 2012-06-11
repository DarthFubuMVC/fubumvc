using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class when_using_the_WebFormsEndpoint_attribute
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<FakeController>();
            });
        }

        #endregion

        private BehaviorGraph _graph;
        private IConfigurationObserver _observer;

        public class FakeController
        {
            [WebFormsEndpoint(typeof (FakeView))]
            public FakeOutput DoNothing(FakeInput input)
            {
                return null;
            }
        }

        public class FakeInput
        {
        }

        public class FakeOutput
        {
        }

        public class FakeView : FubuPage<FakeOutput>
        {
        }


        [Test]
        public void observer_should_record_call_status()
        {
            var token = new WebFormViewToken(typeof (FakeView));
            var actions = _graph.Actions().Where(x => !x.HasAnyOutputBehavior()
                                                      && x.Method.HasAttribute<WebFormsEndpointAttribute>()).ToList();
            for (var index = 0; index < actions.Count; index++)
            {
                var call = actions[index];
                _observer.AssertWasCalled(o => o.RecordCallStatus(Arg<ActionCall>.Is.Equal(call),
                                                                  Arg<string>.Matches(s => s.Contains(
                                                                      "Action '{0}' has {1} declared, using WebForms view '{2}'"
                                                                          .ToFormat(call.Description,
                                                                                    "WebFormsEndpointAttribute", token)))));
            }
        }
    }
}