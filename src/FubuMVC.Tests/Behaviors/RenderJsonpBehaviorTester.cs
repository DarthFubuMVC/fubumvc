using FubuCore.Binding;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Runtime;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class when_rendering_json_from_a_model_object : InteractionContext<JsonpAwareWriter>
    {
        private JsonOutput output;
        private InMemoryRequestData requestData;
        private InMemoryOutputWriter theOutputWriter;
        
        protected override void beforeEach()
        {
            output = new JsonOutput
                         {
                             Name = "Max",
                             Age = 6
                         };

            requestData = new InMemoryRequestData();
            Services.Inject<IRequestData>(requestData);

            theOutputWriter = new InMemoryOutputWriter();
            Services.Inject<IOutputWriter>(theOutputWriter);
        }

        [Test]
        public void should_write_json_mimetype()
        {
            ClassUnderTest.Write(output);

            theOutputWriter.ContentType.ShouldEqual(MimeType.Json.ToString());
        }

        [Test]
        public void should_write_only_json_output_when_jsonp_request_parameter_is_not_present()
        {
            ClassUnderTest.Write(output);

            var json = JsonUtil.ToJson(output);
            theOutputWriter.ToString().TrimEnd().ShouldEqual(json);
        }

        [Test]
        public void should_write_json_padded_output_when_jsonp_request_parameter_is_present()
        {
            const string padding = "parseResult";
            requestData[JsonpAwareWriter.JsonPHttpRequest] = padding;
            
            ClassUnderTest.Write(output);

            var expectedResult = padding + "(" + JsonUtil.ToJson(output) + ");";
            theOutputWriter.ToString().TrimEnd().ShouldEqual(expectedResult);
        }
    }
}