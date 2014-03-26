using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements.Builders
{
    [TestFixture]
    public class AddNameModifierTester
    {
        private ElementRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theRequest = ElementRequest.For<Address>(x => x.Address1);
            theRequest.ElementId = "TheElementId";
        }

        [Test]
        public void do_nothing_if_it_is_not_an_input_element()
        {
            theRequest.ReplaceTag(new HtmlTag("div"));

            new AddNameModifier().Modify(theRequest);

            theRequest.CurrentTag.HasAttr("name").ShouldBeFalse();
        }

        [Test]
        public void add_the_name_equal_to_the_element_id_if_it_is_an_input_element()
        {
            theRequest.ReplaceTag(new TextboxTag());

            new AddNameModifier().Modify(theRequest);

            theRequest.CurrentTag.Attr("name").ShouldEqual(theRequest.ElementId);
        }
    }
}