using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class actions_that_return_IDictionary_are_json
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<MyController>();

            theGraph = registry.BuildGraph();
        }

        [Test]
        public void methods_that_return_an_IDictionary_string_object_should_be_asymmetric_json()
        {
            theGraph.BehaviorFor<MyController>(x => x.ReturnsJson(null)).IsAsymmetricJson().ShouldBeTrue();
            var behaviorChain = theGraph.BehaviorFor<MyController>(x => x.ReturnOtherJson());

            behaviorChain.ResourceType().ShouldEqual(typeof (IDictionary<string, object>));
            behaviorChain.IsAsymmetricJson().ShouldBeTrue();
        }

        [Test]
        public void methods_that_do_not_return_idictionary_should_not_be_impacted()
        {
            theGraph.BehaviorFor<MyController>(x => x.NotJson()).IsAsymmetricJson().ShouldBeFalse();
        }

        public class Input1{}
        public class MyController
        {
            public IDictionary<string, object> ReturnsJson(Input1 input)
            {
                return  new Dictionary<string, object>();
            }

            public IDictionary<string, object> ReturnOtherJson()
            {
                return new Dictionary<string, object>();
            }

            public Input1 NotJson()
            {
                return null;
            }
        }
    }
}