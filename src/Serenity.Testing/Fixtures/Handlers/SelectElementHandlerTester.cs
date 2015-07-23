using Shouldly;
using HtmlTags;
using HtmlTags.Extended.Attributes;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures.Handlers;

namespace Serenity.Testing.Fixtures.Handlers
{
    public class SelectElementHandlerTester<TBrowser> : ScreenManipulationTester<TBrowser> where TBrowser : IBrowserLifecycle, new()
    {
        private readonly SelectElementHandler _handler = new SelectElementHandler();

        private const string A = "a";
        private const string B = "b";
        private const string C = "c";

        private const string Select1Id = "select1";
        private const string Select2Id = "select2";
        private const string Select3Id = "select3";
        private const string Select4Id = "select4";

        private readonly By _select1ById = By.Id(Select1Id);
        private readonly By _select2ById = By.Id(Select2Id);
        private readonly By _select3ById = By.Id(Select3Id);
        private readonly By _select4ById = By.Id(Select4Id);

        protected override void ConfigureDocument(HtmlDocument document)
        {
            const string option = "option";
            const string selected = "selected";
            const string valueAttr = "value";

            const int two = 2;
            const int three = 3;

            document.Add(new SelectTag(tag =>
            {
                tag.Option(A, A);
                tag.Option(B, two.ToString()).Id(B);
                tag.Option(C, three).Id(C);

                tag.SelectByValue(two.ToString());
            }).Name(Select1Id).Id(Select1Id));

            document.Add(new SelectTag(tag =>
            {
                tag.Option(A, A);
                tag.Option(B, two).Id(B);
                tag.Option(C, three).Id(C);

            }).Name(Select2Id).Id(Select2Id));

            document.Add(new SelectTag(tag =>
            {
                tag.Add(option).Text(A);
                tag.Add(option).Text(B).Attr(selected, selected);
                tag.Add(option).Text(C);

            }).Name(Select3Id).Id(Select3Id));

            document.Add(new SelectTag(tag =>
            {
                tag.Option(A, A);
                tag.Option(B, two).Id(B);
                tag.Option(C, three).Id(C);
                tag.Option(valueAttr, B).Id(C);

                tag.SelectByValue(two);
            }).Name(Select4Id).Id(Select4Id));
        }

        [Test]
        public void get_data_with_selected_option_with_both_value_and_text()
        {
            var select1 = Driver.FindElement(_select1ById);
            const string result = "b=2";

            _handler.GetData(null, select1).ShouldBe(result);
        }

        [Test]
        public void get_data_with_selected_option_that_has_only_text()
        {
            var select3 = Driver.FindElement(_select3ById);
            const string result = "b=b";

            _handler.GetData(null, select3).ShouldBe(result);
        }

        [Test]
        public void match_against_value_is_default()
        {
            var select1 = Driver.FindElement(_select1ById);
            const int expectTrue = 2;
            const int expectFalse = 3;

            _handler.MatchesData(select1, expectTrue).ShouldBeTrue();
            _handler.MatchesData(select1, expectFalse).ShouldBeFalse();
        }

        [Test]
        public void can_fall_thru_to_checking_against_display_if_value_does_not_exist()
        {
            var select1 = Driver.FindElement(_select1ById);
            const string expectTrue = "b";
            const string expectFalse = "c";

            _handler.MatchesData(select1, expectTrue).ShouldBeTrue();
            _handler.MatchesData(select1, expectFalse).ShouldBeFalse();
        }

        [Test]
        public void match_by_display_if_value_does_not_exist()
        {
            var select1 = Driver.FindElement(_select1ById);
            const string expectTrue = "b";
            const string expectFalse = "c";

            _handler.MatchesData(select1, expectTrue).ShouldBeTrue();
            _handler.MatchesData(select1, expectFalse).ShouldBeFalse();
        }

        [Test]
        public void does_not_fall_thru_to_display_if_the_expected_value_is_a_value()
        {
            var select4 = Driver.FindElement(_select4ById);
            const string expectFalse = "b";

            _handler.MatchesData(select4, expectFalse).ShouldBeFalse();
        }

        [Test]
        public void matches_has_to_be_false_when_nothing_is_selected()
        {
            var nothingSelectedElement = Driver.FindElement(_select2ById);
            const string expectFalse = "anything";

            _handler.MatchesData(nothingSelectedElement, expectFalse).ShouldBeFalse();
        }
    }
}
