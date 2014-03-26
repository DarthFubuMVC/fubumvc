using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements.Builders
{
    [TestFixture]
    public class DefaultLabelBuilderTester
    {
        [Test]
        public void should_consider_case_changes_as_word_boundaries()
        {
            DefaultLabelBuilder.BreakUpCamelCase("DateEntered").ShouldEqual("Date Entered");
        }

        [Test]
        public void should_consider_numbers_as_word_boundaries()
        {
            DefaultLabelBuilder.BreakUpCamelCase("The1Day2").ShouldEqual("The 1 Day 2");
        }

        [Test]
        public void should_not_consider_consecutive_numbers_as_word_boundaries()
        {
            DefaultLabelBuilder.BreakUpCamelCase("Address22").ShouldEqual("Address 22");
        }

        [Test]
        public void should_not_consider_consecutive_numbers_between_words_as_word_boundaries_()
        {
            DefaultLabelBuilder.BreakUpCamelCase("Address223City").ShouldEqual("Address 223 City");
        }

        [Test]
        public void should_consider_underscores_as_word_boundaries()
        {
            DefaultLabelBuilder.BreakUpCamelCase("Date_Entered").ShouldEqual("Date Entered");
        }

        [Test]
        public void build_label()
        {
            var request = ElementRequest.For<Address>(x => x.Address1);
            request.ElementId = "Address1Id";

            new DefaultLabelBuilder().Build(request)
                .ToString()
                .ShouldEqual("<label for=\"Address1Id\">Address 1</label>");
        }
    }
}