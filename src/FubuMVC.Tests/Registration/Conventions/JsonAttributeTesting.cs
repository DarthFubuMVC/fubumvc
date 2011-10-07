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
    public class JsonAttributeTesting
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
        public void symmetric_json_attribute_makes_the_input_json_only()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Symmetric(null));
            var theInput = theChain.ConnegInputNode();
            theInput.AllowHttpFormPosts.ShouldBeFalse();
            theInput.SelectedFormatterTypes.Single().ShouldEqual(typeof (JsonFormatter));
        }

        [Test]
        public void symmetric_json_attribute_makes_the_output_json_only()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Symmetric(null));
            var theOutput = theChain.ConnegOutputNode();
            theOutput.SelectedFormatterTypes.Single().ShouldEqual(typeof(JsonFormatter));
        }

        [Test]
        public void asymmetric_json_attribute_makes_the_input_take_html_or_json()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Assymmetric(null));
            var theInput = theChain.ConnegInputNode();
            theInput.AllowHttpFormPosts.ShouldBeTrue();
            theInput.SelectedFormatterTypes.Single().ShouldEqual(typeof(JsonFormatter)); 
        }

        [Test]
        public void asymmetric_json_attribute_makes_the_output_json_only()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Assymmetric(null));
            var theOutput = theChain.ConnegOutputNode();
            theOutput.SelectedFormatterTypes.Single().ShouldEqual(typeof(JsonFormatter));
        }

        public class JsonController
        {
            [SymmetricJson]
            public Output1 Symmetric(Input1 input)
            {
                return null;
            }

            [AsymmetricJson]
            public Output2 Assymmetric(Input2 input)
            {
                return null;
            }
        }

        public class Input1
        {
        }

        public class Input2
        {
        }

        public class Input3
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