using System;
using System.Linq;
using System.Linq.Expressions;
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
                x.Actions.IncludeTypesImplementing<JsonOutputAttachmentTesterController>();

                x.Output.ToHtml.WhenCallMatches(
                    call => call.OutputType() == typeof (string) && call.Method.Name.ToLower().Contains("html"));
                x.Output.ToJson.WhenTheOutputModelIs<CrudReport>().WhenTheOutputModelIs<ContinuationClass>();
                x.Output.To(c => new RenderHtmlTagNode()).WhenTheOutputModelIs<HtmlTagOutput>();
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
            graph.BehaviorChainCount.ShouldEqual(20);
            graph.Behaviors.Where(chain => chain.Any(x => x is RenderJsonNode)).Select(x => x.Calls.First().Method.Name)
                .ShouldHaveTheSameElementsAs("Report", "Report2", "WhatNext", "Decorated", "OutputJson1", "OutputJson2", "OutputJson3");
        }

        [Test]
        public void use_the_filter_for_html_output()
        {
            BehaviorNode behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.StringifyHtml()).Calls.First().Next;

            behavior.ShouldBeOfType<RenderTextNode<string>>().MimeType.ShouldEqual(MimeType.Html);
        }

        private BehaviorChain chainFor(Expression<Action<JsonOutputAttachmentTesterController>> expression)
        {
            return graph.BehaviorFor(expression);
        }

        private BehaviorChain chainFor(Expression<Func<JsonOutputAttachmentTesterController, object>> expression)
        {
            return graph.BehaviorFor(expression);
        }

        [Test]
        public void methods_that_take_in_a_json_message_class_should_have_the_json_deserialization_behavior_in_front_of_the_action_call()
        {
            chainFor(x => x.JsonInput1(null)).FirstCall().Previous.ShouldEqual(new DeserializeJsonNode(typeof (Json1)));
            chainFor(x => x.JsonInput2(null)).FirstCall().Previous.ShouldEqual(new DeserializeJsonNode(typeof(Json2)));
            chainFor(x => x.JsonInput3(null)).FirstCall().Previous.ShouldEqual(new DeserializeJsonNode(typeof(Json3)));
        
            
        }

        [Test]
        public void methods_that_do_not_take_in_a_json_message_should_not_have_a_json_deserialization_behavior()
        {
            chainFor(x => x.NotJson1(null)).Any(x => x is DeserializeJsonNode).ShouldBeFalse();
            chainFor(x => x.NotJson2(null)).Any(x => x is DeserializeJsonNode).ShouldBeFalse();
            chainFor(x => x.NotJson3(null)).Any(x => x is DeserializeJsonNode).ShouldBeFalse();
        }

        [Test]
        public void methods_that_return_a_json_message_should_output_json()
        {
            BehaviorChain chain = chainFor(x => x.OutputJson1());
            chain.Any(x => x.GetType() == typeof(RenderJsonNode)).ShouldBeTrue();
            chainFor(x => x.OutputJson2()).Any(x => x.GetType() == typeof(RenderJsonNode)).ShouldBeTrue();
            chainFor(x => x.OutputJson3()).Any(x => x.GetType() == typeof(RenderJsonNode)).ShouldBeTrue();
        }

        [Test]
        public void methods_that_do_not_return_a_json_message_should_not_output_json()
        {
            chainFor(x => x.NotOutputJson1()).Any(x => x.GetType() == typeof(RenderJsonNode)).ShouldBeFalse();
            chainFor(x => x.NotOutputJson2()).Any(x => x.GetType() == typeof(RenderJsonNode)).ShouldBeFalse();
            chainFor(x => x.NotOutputJson3()).Any(x => x.GetType() == typeof(RenderJsonNode)).ShouldBeFalse();
        }

        [Test]
        public void methods_that_return_html_tag_should_output_html_tag()
        {
            var chain = chainFor(x=>x.GetFake());
            chain.Any(x => x.GetType() == typeof(RenderHtmlTagNode)).ShouldBeTrue();
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

        public HtmlTagOutput GetFake(){return null;}

        [JsonEndpoint]
        public ViewModel1 Decorated()
        {
            return null;
        }

        public void JsonInput1(Json1 json){}
        public void JsonInput2(Json2 json){}
        public void JsonInput3(Json3 json){}

        public void NotJson1(NotJson1 message){}
        public void NotJson2(NotJson2 message){}
        public void NotJson3(NotJson3 message){}

        public Json1 OutputJson1(){return new Json1();}
        public Json2 OutputJson2(){return new Json2();}
        public Json3 OutputJson3(){return new Json3();}
    
        public NotJson1 NotOutputJson1(){return new NotJson1();}
        public NotJson2 NotOutputJson2(){return new NotJson2();}
        public NotJson3 NotOutputJson3(){return new NotJson3();}
    }

    public class Json1 : JsonMessage{}
    public class Json2 : JsonMessage{}
    public class Json3 : JsonMessage{}

    public class NotJson1{}
    public class NotJson2{}
    public class NotJson3{}

    public class HtmlTagOutput{}
}