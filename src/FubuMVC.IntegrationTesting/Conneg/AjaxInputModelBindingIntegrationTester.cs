using System;
using System.Net;
using System.Reflection;
using FubuCore.Binding;
using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class AjaxInputModelBindingIntegrationTester 
    {

        [Test]
        public void The_answer_should_be_model_bound_when_posting_json()
        {
            var theInput = new JsonInput
            {
                GoodThing1 = "Obiwan",
                GoodThing2 = "Han Solo"
            };

            var output = TestHost.Scenario(_ =>
            {
                _.Post.Json(theInput);
                _.StatusCodeShouldBeOk();

             
            })
            .Body.ReadAsJson<JsonInput>();

            output.GoodThing1.ShouldBe("Obiwan");
            output.GoodThing2.ShouldBe("Han Solo");
            output.TheAnswerToLifeTheUniverseAndEverything.ShouldBe(42);
        }

        [Test]
        public void The_answer_should_be_model_bound_when_posting_xml()
        {
            var theInput = new XmlInput
            {
                BadThing1 = "Vader",
                BadThing2 = "Jabba"
            };

            var output = TestHost.Scenario(_ =>
            {
                _.Post.Xml(theInput);
            }).Body.ReadAsXml<XmlInput>();

            output.BadThing1.ShouldBe("Vader");
            output.BadThing2.ShouldBe("Jabba");
            output.TheAnswerToLifeTheUniverseAndEverything.ShouldBe(42);
        }

        [Test]
        public void The_answer_should_be_overwritten_by_binding_even_if_already_supplied_in_json_input()
        {
            var theInput = new JsonInput
            {
                GoodThing1 = "Obiwan",
                GoodThing2 = "Han Solo",
                TheAnswerToLifeTheUniverseAndEverything = 7
            };

            var output = TestHost.Scenario(_ =>
            {
                _.Post.Json(theInput);
            }).Body.ReadAsJson<JsonInput>();

            output.TheAnswerToLifeTheUniverseAndEverything.ShouldBe(42);
        }

        [Test]
        public void The_answer_should_be_overwritten_by_binding_even_if_already_supplied_in_xml_input()
        {
            var theInput = new XmlInput
            {
                BadThing1 = "Vader",
                BadThing2 = "Jabba",
                TheAnswerToLifeTheUniverseAndEverything = 13
            };

            var output = TestHost.Scenario(_ =>
            {
                _.Post.Xml(theInput);
            }).Body.ReadAsXml<XmlInput>();

            output.TheAnswerToLifeTheUniverseAndEverything.ShouldBe(42);
        }
    }


    public class JsonEndpoint
    {
        public JsonInput Json(JsonInput input)
        {
            return input;
        }
    }


    public class XmlEndpoint
    {
        public XmlInput Xml(XmlInput input)
        {
            return input;
        }
    }


    public interface IAjaxInput
    {
    }


    public class JsonInput : IAjaxInput
    {
        public string GoodThing1 { get; set; }
        public string GoodThing2 { get; set; }

        public int TheAnswerToLifeTheUniverseAndEverything { get; set; }
    }


    public class XmlInput : IAjaxInput
    {
        public string BadThing1 { get; set; }
        public string BadThing2 { get; set; }

        public int TheAnswerToLifeTheUniverseAndEverything { get; set; }
    }


    public class TheAnswerBinder : IPropertyBinder
    {
        public bool Matches(PropertyInfo property)
        {
            return (property.PropertyType == typeof (int) &&
                    property.Name.StartsWith("TheAnswer", StringComparison.InvariantCultureIgnoreCase));
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            property.SetValue(context.Object, 42);
        }
    }
}