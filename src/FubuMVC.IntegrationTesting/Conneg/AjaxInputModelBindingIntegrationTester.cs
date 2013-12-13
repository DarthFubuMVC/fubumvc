using System;
using System.Net;
using System.Reflection;

using FubuCore.Binding;

using FubuMVC.Core;
using FubuMVC.TestingHarness;

using FubuTestingSupport;

using NUnit.Framework;


namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class AjaxInputModelBindingIntegrationTester : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<JsonEndpoint>();
            registry.Actions.IncludeType<XmlEndpoint>();
            registry.Routes.ConstrainToHttpMethod(call => true, "POST");
            registry.Services(r => r.AddService<IPropertyBinder, TheAnswerBinder>());
        }


        [Test]
        public void The_answer_should_be_model_bound_when_posting_json()
        {
            var theInput = new JsonInput
            {
                GoodThing1 = "Obiwan",
                GoodThing2 = "Han Solo"
            };

            var output = endpoints.PostJson(theInput)
                                  .StatusCodeShouldBe(HttpStatusCode.OK)
                                  .ReadAsJson<JsonInput>();

            output.GoodThing1.ShouldEqual("Obiwan");
            output.GoodThing2.ShouldEqual("Han Solo");
            output.TheAnswerToLifeTheUniverseAndEverything.ShouldEqual(42);
        }

        [Test]
        public void The_answer_should_be_model_bound_when_posting_xml()
        {
            var theInput = new XmlInput
            {
                BadThing1 = "Vader",
                BadThing2 = "Jabba"
            };

            var output = endpoints.PostXml(theInput)
                                  .StatusCodeShouldBe(HttpStatusCode.OK)
                                  .ReadAsJson<XmlInput>();

            output.BadThing1.ShouldEqual("Vader");
            output.BadThing2.ShouldEqual("Jabba");
            output.TheAnswerToLifeTheUniverseAndEverything.ShouldEqual(42);
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

            var output = endpoints.PostJson(theInput)
                                  .StatusCodeShouldBe(HttpStatusCode.OK)
                                  .ReadAsJson<JsonInput>();

            output.TheAnswerToLifeTheUniverseAndEverything.ShouldEqual(42);
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

            var output = endpoints.PostXml(theInput)
                                  .StatusCodeShouldBe(HttpStatusCode.OK)
                                  .ReadAsJson<XmlInput>();

            output.TheAnswerToLifeTheUniverseAndEverything.ShouldEqual(42);
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
    {}


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
            return (property.PropertyType == typeof(int) && property.Name.StartsWith("TheAnswer", StringComparison.InvariantCultureIgnoreCase));
        }

        public void Bind(PropertyInfo property, IBindingContext context)
        {
            property.SetValue(context.Object, 42);
        }
    }
}