using Shouldly;
using HtmlTags;
using HtmlTags.Extended.Attributes;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures.Handlers;

namespace Serenity.Testing.Fixtures.Handlers
{
    public class TextboxElementHandlerTester<TBrowser> : ScreenManipulationTester<TBrowser> where TBrowser : IBrowserLifecycle, new()
    {
        private readonly TextboxElementHandler _handler = new TextboxElementHandler();

        private const string TheText = "(100)444-9898";

        private const string TextboxId = "textbox1";
        private const string SelectId = "select1";
        private const string SubmitId = "submit1";
        private const string PasswordTextboxId = "pwdtextbox1";

        private readonly By _textboxById = By.Id(TextboxId);
        private readonly By _selectById = By.Id(SelectId);
        private readonly By _submitById = By.Id(SubmitId);
        private readonly By _passwordTextboxById = By.Id(PasswordTextboxId);

        protected override void ConfigureDocument(HtmlDocument document)
        {
            document.Add(new HtmlTag("input", tag =>
            {
                tag.Value(TheText);
                tag.Id(TextboxId);
            }));

            document.Add(new SelectTag(tag =>
            {
                tag.Id(SelectId);
            }));

            document.Add(new HtmlTag("input", tag =>
            {
                tag.Value("Submit button");
                tag.Id(SubmitId);
                tag.Attr("type", "submit");
            }));

            document.Add(new HtmlTag("input", tag =>
            {
                tag.Value(TheText);
                tag.Id(PasswordTextboxId);
                tag.Attr("type", "password");
            }));
        }

        [Test]
        public void should_be_able_to_clear_original_text()
        {
            var textbox1 = Driver.FindElement(_textboxById);
            _handler.EraseData(null, textbox1);
            _handler.GetData(null, textbox1).ShouldBeEmpty();
        }

        [Test]
        public void should_be_able_to_clear_original_text_password()
        {
            var textbox1 = Driver.FindElement(_passwordTextboxById);
            _handler.EraseData(null, textbox1);
            _handler.GetData(null, textbox1).ShouldBeEmpty();
        }

        [Test]
        public void should_be_able_to_get_text_from_field()
        {
            var textbox1 = Driver.FindElement(_textboxById);
            _handler.GetData(null, textbox1).ShouldBe(TheText);
        }

        [Test]
        public void should_be_able_to_get_text_from_field_password()
        {
            var textbox1 = Driver.FindElement(_passwordTextboxById);
            _handler.GetData(null, textbox1).ShouldBe(TheText);
        }

        [Test]
        public void should_be_able_to_write_to_a_clean_field()
        {
            const string input = "Index There";
            var textbox1 = Driver.FindElement(_textboxById);
            _handler.EnterData(null, textbox1, input);
            _handler.GetData(null, textbox1).ShouldBe(input);
        }

        [Test]
        public void should_be_able_to_write_to_a_clean_field_password()
        {
            const string input = "Index There";
            var textbox1 = Driver.FindElement(_passwordTextboxById);
            _handler.EnterData(null, textbox1, input);
            _handler.GetData(null, textbox1).ShouldBe(input);
        }

        [Test]
        public void should_match_textbox()
        {
            var element = Driver.FindElement(_textboxById);
            _handler.Matches(element).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_select()
        {
            var element = Driver.FindElement(_selectById);
            _handler.Matches(element).ShouldBeFalse();
        }

        [Test]
        public void should_not_match_submit()
        {
            var element = Driver.FindElement(_submitById);
            _handler.Matches(element).ShouldBeFalse();
        }

        [Test]
        public void should_match_password_textbox()
        {
            var element = Driver.FindElement(_passwordTextboxById);
            _handler.Matches(element).ShouldBeTrue();
        }
    }
}
