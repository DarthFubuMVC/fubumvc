using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Conneg
{
    [TestFixture]
    public class Output_from_actions_that_return_ajax_continuations 
    {

        [Test]
        public void get_output_from_continuation()
        {
            TestHost.Scenario(_ =>
            {
                _.Post.Json(new AjaxInput
                {
                    Message = "something"
                });

                _.ContentShouldBe("{\"success\":true,\"refresh\":false,\"message\":\"something\"}");
                _.ContentTypeShouldBe(MimeType.Json);
            });
        }

        [Test]
        public void get_output_from_custom_continuation()
        {
            var specialInput = new SpecialInput {Name = "somebody"};

            TestHost.Scenario(_ =>
            {
                _.Post.Json(specialInput);
                _.ContentShouldBe("{\"success\":false,\"refresh\":false,\"name\":\"somebody\"}");
                _.ContentTypeShouldBe(MimeType.Json);
            });
        }
    }

    public class AjaxContinuationEndpoints
    {
        public AjaxContinuation post_input(AjaxInput input)
        {
            return new AjaxContinuation
            {
                Success = true,
                Message = input.Message
            };
        }

        public MySpecialContinuation post_special(SpecialInput input)
        {
            return new MySpecialContinuation
            {
                Name = input.Name
            };
        }
    }

    public class SpecialInput
    {
        public string Name { get; set; }
    }

    public class MySpecialContinuation : AjaxContinuation
    {
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }
    }

    public class AjaxInput
    {
        public string Message { get; set; }
    }
}