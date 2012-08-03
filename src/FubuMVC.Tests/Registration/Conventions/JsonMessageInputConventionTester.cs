using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class JsonMessageInputConventionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<JsonController>();
            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        #endregion

        private BehaviorGraph theGraph;

        public class JsonController
        {
            public Output1 NonJson(Input1 input)
            {
                return null;
            }

            [FubuPartial]
            public Output2 NonJsonPartial(Input2 input)
            {
                return null;
            }

            public Output3 Json(Input3 input)
            {
                return null;
            }
        }

        public class Input1
        {
        }

        public class Input2 : JsonMessage
        {
        }

        public class Input3 : JsonMessage
        {
        }

        public class Output1
        {
        }

        public class Output2
        {
        }

        public class Output3
        {
        }

        [Test]
        public void JsonMessage_interface_has_no_effect_for_partial_actions()
        {
            var theChain = theGraph.BehaviorFor(typeof (Input2));
            theChain.Input.UsesFormatter<JsonFormatter>().ShouldBeFalse();
            theChain.Output.UsesFormatter<JsonFormatter>().ShouldBeFalse();
        }

        [Test]
        public void JsonMessage_interface_makes_the_input_asymmetric_json()
        {
            var theChain = theGraph.BehaviorFor(typeof (Input3));
            var theInput = theChain.Input;

            theChain.IsAsymmetricJson().ShouldBeTrue();
        }

        [Test]
        public void action_without_json_attributes_or_JsonMessage_input_model_has_no_conneg()
        {
            // You have to make the endpoint get some sort of reader/writer to test the negative case,
            // otherwise the default "use json & xml if nothing else is provided" convention
            // takes over
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<JsonController>();
            registry.Configure(graph =>
            {
                graph.Behaviors.Where(x => x.InputType() == typeof (Input1)).Each(chain =>
                {
                    chain.Input.AddFormatter<XmlFormatter>();
                    chain.Output.AddFormatter<XmlFormatter>();
                });
            });

            theGraph = BehaviorGraph.BuildFrom(registry);

            var theChain = theGraph.BehaviorFor(typeof (Input1));

            theChain.Input.UsesFormatter<JsonFormatter>().ShouldBeFalse();
            theChain.Output.UsesFormatter<JsonFormatter>().ShouldBeFalse();
        }
    }
}