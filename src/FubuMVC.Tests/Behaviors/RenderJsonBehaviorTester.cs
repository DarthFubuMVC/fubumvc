using System;
using System.Net.Mime;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using InMemoryRequestData=FubuMVC.Core.Runtime.InMemoryRequestData;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class when_rendering_json_from_a_model_object_using_default_json_writer : InteractionContext<JsonWriter>
    {
        private JsonOutput output;
        private InMemoryRequestData requestData;
        private string mimeType;
        private string rawOutput;

        protected override void beforeEach()
        {
            output = new JsonOutput
            {
                Name = "Max",
                Age = 6
            };

            requestData = new InMemoryRequestData();
            Services.Inject<IRequestData>(requestData);

            mimeType = null;
            rawOutput = null;

            MockFor<IOutputWriter>().Stub(x => x.Write(null, null)).IgnoreArguments().Do(
                new Action<string, string>((t, o) =>
                {
                    mimeType = t;
                    rawOutput = o;
                }));

            requestData["X-Requested-With"] = "XMLHttpRequest";

            ClassUnderTest.Write(output);
        }

        [Test]
        public void should_write_json_serialized_string_to_the_output_writer_regardless_of_request_headers()
        {
            string json = JsonUtil.ToJson(output);
            mimeType.ShouldEqual(MimeType.Json.ToString());
            rawOutput.ShouldEqual(json);
        }
    }



    [TestFixture]
    public class when_requesting_json_outside_of_an_ajax_request_using_ajax_aware_writer : InteractionContext<AjaxAwareJsonWriter>
    {
        private JsonOutput output;

        protected override void beforeEach()
        {
            output = new JsonOutput
            {
                Name = "Max",
                Age = 6
            };

            ClassUnderTest.Write(output);
        }

        [Test]
        public void should_write_json_serialized_string_within_an_html_textarea_to_the_output_writer()
        {
            string json = JsonUtil.ToJson(output);
            json = "<html><body><textarea rows=\"10\" cols=\"80\">" + json + "</textarea></body></html>";
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Write(MediaTypeNames.Text.Html, json));
        }
    }

    [TestFixture]
    public class when_rendering_json_from_a_model_object_using_ajax_aware_writer : InteractionContext<AjaxAwareJsonWriter>
    {
        private JsonOutput output;
        private InMemoryRequestData requestData;
        private string mimeType;
        private string rawOutput;

        protected override void beforeEach()
        {
            output = new JsonOutput
            {
                Name = "Max",
                Age = 6
            };

            requestData = new InMemoryRequestData();
            Services.Inject<IRequestData>(requestData);

            mimeType = null;
            rawOutput = null;

            MockFor<IOutputWriter>().Stub(x => x.Write(null, null)).IgnoreArguments().Do(
                new Action<string, string>((t, o) =>
                {
                    mimeType = t;
                    rawOutput = o;
                }));

            requestData["X-Requested-With"] = "XMLHttpRequest";

            ClassUnderTest.Write(output);
        }

        [Test]
        public void should_detect_ajax_request_with_case_insensitivity()
        {
            requestData["X-Requested-With"] = "xmlHtTpReQuest";
            mimeType = null;

            ClassUnderTest.Write(output);

            mimeType.ShouldEqual(MimeType.Json.ToString());
        }
        
        [Test]
        public void should_write_json_serialized_string_to_the_output_writer()
        {
            string json = JsonUtil.ToJson(output);
            mimeType.ShouldEqual(MimeType.Json.ToString());
            rawOutput.ShouldEqual(json);
        }
    }

    [TestFixture]
    public class when_rendering_a_json_response :
        InteractionContext<RenderJsonBehavior<JsonOutput>>
    {
        private JsonOutput output;        
        protected override void beforeEach()
        {
            output = new JsonOutput
            {
                Name = "Max",
                Age = 6
            };

            MockFor<IFubuRequest>().Expect(x => x.Get<JsonOutput>()).Return(output);
            MockFor<IJsonWriter>().Expect(x => x.Write(output));
            ClassUnderTest.InsideBehavior = MockFor<IActionBehavior>();
            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_retrieve_the_output_object_from_the_fubu_request()
        {
            VerifyCallsFor<IFubuRequest>();
        }

        [Test]
        public void should_render_output_via_the_json_writer()
        {
            VerifyCallsFor<IJsonWriter>();
        }

        [Test]
        public void should_return_donext_continue()
        {
            ClassUnderTest.InsideBehavior.AssertWasCalled(x=> x.Invoke());
        }

    }

    public class JsonOutput
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}