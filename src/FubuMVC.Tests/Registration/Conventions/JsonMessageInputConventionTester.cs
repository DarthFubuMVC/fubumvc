using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Media.Formatters;
using NUnit.Framework;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class JsonMessageInputConventionTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<JsonController>();
			theGraph = registry.BuildGraph();
        }

		[Test]
		public void action_without_json_attributes_or_JsonMessage_input_model_has_no_conneg()
		{
			var theChain = theGraph.BehaviorFor(typeof(Input1));
			theChain.ConnegInputNode().ShouldBeNull();
			theChain.ConnegOutputNode().ShouldBeNull();
		}

		[Test]
		public void JsonMessage_interface_has_no_effect_for_partial_actions()
		{
			var theChain = theGraph.BehaviorFor(typeof(Input2));
			theChain.ConnegInputNode().ShouldBeNull();
			theChain.ConnegOutputNode().ShouldBeNull();
		}

		[Test]
		public void JsonMessage_interface_makes_the_input_asymmetric_json()
		{
			var theChain = theGraph.BehaviorFor(typeof(Input3));
			var theInput = theChain.ConnegInputNode();
			
			theInput.AllowHttpFormPosts.ShouldBeTrue();
			theInput.SelectedFormatterTypes.Single().ShouldEqual(typeof(JsonFormatter));
			
			var theOutput = theChain.ConnegOutputNode();
			theOutput.SelectedFormatterTypes.Single().ShouldEqual(typeof(JsonFormatter));
		}

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
    }
}