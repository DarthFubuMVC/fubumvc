using System;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Runtime;
using NUnit.Framework;

namespace IntegrationTesting.Conneg
{
    [TestFixture]
    public class Output_from_actions_that_return_ajax_continuations : FubuRegistryHarness
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<AjaxController>();
        }

        [Test]
        public void get_output_from_continuation()
        {
            endpoints.PostJson(new AjaxInput{
                Message = "something"
            })
                .ContentShouldBe(MimeType.Json, "{\"success\":true,\"refresh\":false,\"message\":\"something\"}");
        }

        [Test]
        public void get_output_from_custom_continuation()
        {
            endpoints.PostJson(new SpecialInput{Name = "somebody"})
                .ContentShouldBe(MimeType.Json, "{\"success\":false,\"refresh\":false,\"name\":\"somebody\"}");
        }
    }

    public class AjaxController
    {
        public AjaxContinuation post_input(AjaxInput input)
        {
            return new AjaxContinuation{
                Success = true,
                Message = input.Message
            };
        }

        public MySpecialContinuation post_special(SpecialInput input)
        {
            return new MySpecialContinuation{
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
        public string Message { get; set;}
    }
}