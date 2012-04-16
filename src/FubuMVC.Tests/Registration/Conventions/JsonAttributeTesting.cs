using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class JsonAttributeTesting
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<JsonController>();

            theGraph = registry.BuildGraph();
        }

        #endregion

        private BehaviorGraph theGraph;

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

        [Test]
        public void asymmetric_json_attribute_makes_the_input_take_html_or_json()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Assymmetric(null));
            var theInput = theChain.Input;
            theInput.AllowHttpFormPosts.ShouldBeTrue();

            theInput.Readers.Count().ShouldEqual(1);
            theInput.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void asymmetric_json_attribute_makes_the_output_json_only()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Assymmetric(null));
            var theOutput = theChain.Output;

            theOutput.UsesFormatter<JsonFormatter>().ShouldBeTrue();
            theOutput.Writers.Count().ShouldEqual(1);
        }

        [Test]
        public void symmetric_json_attribute_makes_the_input_json_only()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Symmetric(null));
            var theInput = theChain.Input;
            theInput.AllowHttpFormPosts.ShouldBeFalse();

            theInput.Readers.Count().ShouldEqual(1);
            theInput.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void symmetric_json_attribute_makes_the_output_json_only()
        {
            var theChain = theGraph.BehaviorFor<JsonController>(x => x.Symmetric(null));
            var theOutput = theChain.Output;

            theOutput.UsesFormatter<JsonFormatter>().ShouldBeTrue();
            theOutput.Writers.Count().ShouldEqual(1);
        }
    }
}