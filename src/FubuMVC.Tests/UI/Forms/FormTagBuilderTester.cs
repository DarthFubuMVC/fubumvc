using FubuMVC.Core.UI.Forms;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Forms
{
    public class FormTagBuilderTester : InteractionContext<FormTagBuilder>
    {
        [Test]
        public void AlwaysMatches()
        {
            ClassUnderTest.Matches(null).ShouldBeTrue();
        }

        [Test]
        public void BuildsFormTag()
        {
            ClassUnderTest.Build(new FormRequest(null, null)).TagName().ShouldEqual("form");
        }

        [Test]
        public void DoesNotCloseByDefault()
        {
            ClassUnderTest.Build(new FormRequest(null, null)).HasClosingTag().ShouldBeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ClosesWhenFormRequestSaysClose(bool close)
        {
            ClassUnderTest.Build(new FormRequest(null, null, close)).HasClosingTag().ShouldEqual(close);
        }

        [Test]
        public void SetsActionUrlFromRequest()
        {
            const string url = "url/to/use/for/form";
            ClassUnderTest.Build(new FormRequest(null, null) {Url = url}).Attr("action").ShouldEqual(url);
        }
    }
}