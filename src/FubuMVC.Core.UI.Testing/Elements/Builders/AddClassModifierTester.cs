using FubuHtml.Elements;
using FubuHtml.Elements.Builders;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuHtml.Testing.Elements.Builders
{
    [TestFixture]
    public class AddClassModifierTester
    {
        [Test]
        public void modify_request()
        {
            var modifier = new AddClassModifier("fizz");

            var request = ElementRequest.For<Address>(x => x.Address1);
            request.ReplaceTag(new HtmlTag("div"));
            request.ReplaceTag(new HtmlTag("div"));

            modifier.Modify(request);

            request.CurrentTag.HasClass("fizz").ShouldBeTrue();
            request.OriginalTag.HasClass("fizz").ShouldBeFalse();
        }
    }
}