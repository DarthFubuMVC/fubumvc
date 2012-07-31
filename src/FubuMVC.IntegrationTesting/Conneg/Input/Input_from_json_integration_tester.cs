using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg.Input
{
    [TestFixture]
    public class Input_from_json_integration_tester : FubuRegistryHarness
    {
        private RouteInputRequest theInput;
        private string expectedJson;

        public Input_from_json_integration_tester()
        {
            theInput = new RouteInputRequest()
                           {
                               Id = Guid.NewGuid(),
                               Input = new AjaxInput() {Message = "herp derp"}
                           };
            expectedJson = JsonUtil.ToJson(theInput);
        }

        protected override void configure(Core.FubuRegistry registry)
        {
            registry.Actions.IncludeType<JsonEndpoint>();
            registry.Routes.ForInputTypesOf<IRequestById>(x => x.RouteInputFor(i => i.Id))
                .ConstrainToHttpMethod(x => x.HandlerType.CanBeCastTo<JsonEndpoint>(),"POST");
            registry.Media.ApplyContentNegotiationToActions(x => true);
        }

        [Test]
        public void id_should_be_model_bound_when_posting_json()
        {

            var output = endpoints.PostJson(theInput, new {Input = new AjaxInput() {Message = "herp derp"}}, contentType: "text/json",
                               accept: "text/json")
                .StatusCodeShouldBe(HttpStatusCode.OK)
                .ReadAsJson<RouteInputRequest>();

            output.Id.ShouldEqual(theInput.Id);
            output.Input.Message.ShouldEqual("herp derp");
        }

    }
    public class JsonEndpoint
    {
        public RouteInputRequest RouteInput(RouteInputRequest request)
        {
            return request;
        }
    }

    public class RouteInputRequest : IRequestById, ConnegMessage
    {
        [RouteInput]
        public Guid Id { get; set; }

        public AjaxInput Input { get; set; }
    }

    public class AjaxInput
    {
        public string Message { get; set;}
    }

    public interface IRequestById   
    {
        Guid Id { get; set; }
    }
}