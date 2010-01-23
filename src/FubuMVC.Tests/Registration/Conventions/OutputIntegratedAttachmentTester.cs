using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Behaviors;
using FubuMVC.Tests.View.FakeViews;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class OutputIntegratedAttachmentTester
    {
        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.Applies.ToThisAssembly();
                x.Actions.IncludeTypesImplementing<JsonOutputAttachmentTesterController>();

                x.Output.ToHtml.WhenCallMatches(
                    call => call.OutputType() == typeof (string) && call.Method.Name.ToLower().Contains("html"));
                x.Output.ToJson.WhenTheOutputModelIs<CrudReport>().WhenTheOutputModelIs<ContinuationClass>();
            })
                .BuildGraph();
        }

        private BehaviorGraph graph;

        [Test]
        public void automatically_output_methds_that_are_decorated_with_JsonEndpoint_to_json()
        {
            BehaviorNode behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.Decorated()).Calls.First().Next;
            behavior.ShouldBeOfType<RenderJsonNode>().ModelType.ShouldEqual(typeof (ViewModel1));
        }

        [Test]
        public void automatically_output_methods_that_return_string_as_text_if_there_is_not_output()
        {
            BehaviorNode behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.Stringify()).Calls.First().Next;

            behavior.ShouldBeOfType<RenderTextNode<string>>().MimeType.ShouldEqual(MimeType.Text);
        }

        [Test]
        public void
            only_the_behaviors_with_an_output_model_reflecting_the_json_criteria_specified_in_the_registry_are_output_to_json
            ()
        {
            graph.BehaviorChainCount.ShouldEqual(7);
            graph.Behaviors.Where(chain => chain.Any(x => x is RenderJsonNode)).Select(x => x.Calls.First().Method.Name)
                .ShouldHaveTheSameElementsAs("Report", "Report2", "WhatNext", "Decorated");
        }

        [Test]
        public void use_the_filter_for_html_output()
        {
            BehaviorNode behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.StringifyHtml()).Calls.First().Next;

            behavior.ShouldBeOfType<RenderTextNode<string>>().MimeType.ShouldEqual(MimeType.Html);
        }
    }

    public class CrudReport
    {
    }

    public class SpecialCrudReport : CrudReport
    {
    }

    public class ContinuationClass
    {
    }

    public class JsonOutputAttachmentTesterController
    {
        public CrudReport Report()
        {
            return null;
        }

        public SpecialCrudReport Report2()
        {
            return null;
        }

        public string Stringify()
        {
            return null;
        }

        public string StringifyHtml()
        {
            return null;
        }

        public ContinuationClass WhatNext()
        {
            return null;
        }

        public Output Output()
        {
            return null;
        }

        [JsonEndpoint]
        public ViewModel1 Decorated()
        {
            return null;
        }
    }
}