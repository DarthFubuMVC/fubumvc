using FubuLocalization;
using FubuMVC.Core.UI.Elements;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Localization.Testing
{
    [TestFixture]
    public class when_building_a_label_tag_with_localization
    {
        private string theLocalizedHeaderForTheProperty;
        private ElementRequest theElementRequest;
        private HtmlTag theLabelTag;

        [SetUp]
        public void SetUp()
        {
            theLocalizedHeaderForTheProperty = LocalizationManager.GetHeader<LabelTarget>(x => x.Name);

            theElementRequest = ElementRequest.For<LabelTarget>(new LabelTarget(), x => x.Name);
            theElementRequest.ElementId = "Name";

            theLabelTag = new LabelBuilder().Build(theElementRequest);
        }

        [Test]
        public void should_be_a_label()
        {
            theLabelTag.TagName().ShouldEqual("label");
        }

        [Test]
        public void should_use_the_named_tag_convention_for_the_property_as_the_for_attribute()
        {
            theLabelTag.Attr("for").ShouldEqual(theElementRequest.ElementId);
        }

        [Test]
        public void should_use_localized_header_for_the_property_as_the_text()
        {
            theLabelTag.Text().ShouldEqual(theLocalizedHeaderForTheProperty);
        }

        public class LabelTarget
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string SomeTitle { get; set;}
        }
    }
}