using System;
using System.Net.Mime;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class when_rendering_json_from_a_model_object : InteractionContext<RenderJsonBehavior<JsonOutput>>
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

            MockFor<IFubuRequest>().Expect(x => x.Get<JsonOutput>()).Return(output);
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

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_detect_ajax_request_with_case_insensitivity()
        {
            requestData["X-Requested-With"] = "xmlHtTpReQuest";
            mimeType = null;

            ClassUnderTest.Invoke();

            mimeType.ShouldEqual(MimeType.Json.ToString());
        }

        [Test]
        public void should_retrieve_the_output_object_from_the_fubu_request()
        {
            VerifyCallsFor<IFubuRequest>();
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
    public class when_rendering_html_friendly_json_from_a_model_object :
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

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_retrieve_the_output_object_from_the_fubu_request()
        {
            VerifyCallsFor<IFubuRequest>();
        }

        [Test]
        public void should_write_json_serialized_string_to_the_output_writer()
        {
            string json = JsonUtil.ToJson(output);
            json = "<html><body><textarea rows=\"10\" cols=\"80\">" + json + "</textarea></body></html>";
            MockFor<IOutputWriter>().AssertWasCalled(x => x.Write(MediaTypeNames.Text.Html, json));
        }
    }

    public class JsonOutput
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}