using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.UI;
using FubuMVC.Tests.TestSupport;
using HtmlTags;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class DefaultValidationSummaryWriterTester : InteractionContext<DefaultValidationSummaryWriter>
    {
        [Fact]
        public void builds_the_summary_tag()
        {
            ClassUnderTest.BuildSummary().ToString()
                .ShouldBe("<div class=\"alert alert-error validation-container\" style=\"display:none\"><p>{0}</p><ul class=\"validation-summary\"></ul></div>".ToFormat(ValidationKeys.Summary.ToString()));
        }

        [Fact]
        public void writes_the_summary_tag()
        {
            var theTag = new LiteralTag("testing");
            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Stub(x => x.BuildSummary()).Return(theTag);
            ClassUnderTest.Write(MimeType.Html.Value, Container.GetInstance<FubuRequestContext>(), new ValidationSummary());

            MockFor<IOutputWriter>().AssertWasCalled(x => x.Write(MimeType.Html.ToString(), theTag.ToString()));
        }
    }
}