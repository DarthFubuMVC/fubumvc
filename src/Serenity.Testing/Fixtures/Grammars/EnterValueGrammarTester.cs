using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures.Grammars;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using FubuTestingSupport;
using HtmlTags.Extended.Attributes;

namespace Serenity.Testing.Fixtures.Grammars
{
    public class EnterValueGrammarTester<TBrowser> : ScreenManipulationTester<TBrowser> where TBrowser : IBrowserLifecycle, new()
    {
        protected override void ConfigureDocument(HtmlDocument document)
        {
            document.Add(new TextboxTag("emptytextbox", string.Empty));
            document.Add(new TextboxTag("textbox", "initial"));

            document.Add(new TextboxTag("disabled", "something").Attr("disabled", true));
            document.Add(new TextboxTag("hidden", "something").Style("display", "none"));

            document.Add(new TextboxTag("passwordTextbox", "something").Attr("type", "password"));

            document.Add(new SelectTag(tag =>
            {
                tag.Option("a", "a");
                tag.Option("b", 2).Id("b");
                tag.Option("c", 3).Id("c");
            }).Name("select1"));
        }

        private EnterValueGrammar grammarForName(string name)
        {
            var def = new GestureConfig{
                Template = "enter {data} for textbox",
                Finder = () => Driver.FindElement(By.Name(name)),
                FinderDescription = "name=" + name,
                Description = "Enter data for blah",
                CellName = "data",
                
            };

            return new EnterValueGrammar(def);
        }

        [Test]
        public void select_value_in_select_tag_by_display_first()
        {
            grammarForName("select1").Execute(new Step().With("data", "b"));

            Driver.FindElement(By.Name("select1")).FindElement(By.Id("b")).Selected.ShouldBeTrue();

            grammarForName("select1").Execute(new Step().With("data", "c"));
            Driver.FindElement(By.Name("select1")).FindElement(By.Id("c")).Selected.ShouldBeTrue();
        }

        [Test]
        public void select_option_that_does_not_exist_should_toss_out_a_list_of_all_the_available_options()
        {
            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammarForName("select1").Execute(new Step().With("data", "d"));
            }).ShouldContainErrorMessage("a/a\nb/2\nc/3");
        }

        [Test]
        public void select_value_in_select_tag_by_value_second()
        {
            grammarForName("select1").Execute(new Step().With("data", "2"));

            Driver.FindElement(By.Name("select1")).FindElement(By.Id("b")).Selected.ShouldBeTrue();

            grammarForName("select1").Execute(new Step().With("data", "3"));
            Driver.FindElement(By.Name("select1")).FindElement(By.Id("c")).Selected.ShouldBeTrue();
        }

        [Test]
        public void click_hidden_element_should_throw_exception()
        {
            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammarForName("hidden").Execute();
            }).ShouldContainErrorMessage(ClickGrammar.HiddenElementMessage);
        }

        [Test]
        public void click_disabled_element_should_throw_exception()
        {
            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammarForName("disabled").Execute();
            }).ShouldContainErrorMessage(ClickGrammar.DisabledElementMessage);
        }

        [Test]
        public void enter_text_for_an_empty_textbox()
        {
            grammarForName("emptytextbox").Execute(new Step().With("data", "some text"));

            Driver.FindElement(By.Name("emptytextbox")).GetAttribute("value").ShouldEqual("some text");
        }

        [Test]
        public void enter_text_for_a_textbox_that_is_not_initially_empty()
        {
            grammarForName("textbox").Execute(new Step().With("data", "different"));
            Driver.FindElement(By.Name("textbox")).GetAttribute("value").ShouldEqual("different");
        }

        [Test]
        public void enter_empty_text_into_a_password_textbox()
        {
            grammarForName("passwordTextbox").Execute(new Step().With("data", "EMPTY"));
            Driver.FindElement(By.Name("passwordTextbox")).GetAttribute("value").ShouldBeEmpty();
        }

        [Test]
        public void enter_text_into_a_password_textbox()
        {
            grammarForName("passwordTextbox").Execute(new Step().With("data", "abc"));
            Driver.FindElement(By.Name("passwordTextbox")).GetAttribute("value").ShouldEqual("abc");
        }
    }
}