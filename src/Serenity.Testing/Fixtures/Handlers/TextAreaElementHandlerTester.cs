using Shouldly;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures.Handlers;

namespace Serenity.Testing.Fixtures.Handlers
{
    public class TextAreaElementHandlerTester<TBrowser> : ScreenManipulationTester<TBrowser> where TBrowser : IBrowserLifecycle, new()
    {
        private readonly TextAreaElementHandler _handler = new TextAreaElementHandler();

        private const string TheText = "Test data within the textarea";
        private const string Id = "textbox1";
        private readonly By _byId = By.Id(Id);

        protected override void ConfigureDocument(HtmlDocument document)
        {
            document.Add(new HtmlTag("textarea", tag =>
            {
                tag.Id(Id);
                tag.Text(TheText);
            }));
        }

        [Test]
        public void should_be_able_to_read_write()
        {
            var textarea1 = Driver.FindElement(_byId);
            _handler.EnterData(null, textarea1, "New Data");
            _handler.GetData(null, textarea1).ShouldBe("New Data");
        }

        [Test]
        public void clearing_data_should_return_clear_and_not_original_text()
        {
            var textarea1 = Driver.FindElement(_byId);
            _handler.EnterData(null, textarea1, "");
            _handler.GetData(null, textarea1).ShouldBe("");
        }
    }
}
