using System;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using FubuTestingSupport;

namespace Serenity.Testing.Fixtures
{
    [TestFixture]
    public class EnterDataTester : ScreenManipulationTester
    {
        protected override void configureDocument(HtmlDocument document)
        {
            document.Add(new TextboxTag("emptytextbox", string.Empty));
            document.Add(new TextboxTag("textbox", "initial"));

            document.Add(new TextboxTag("disabled", "something").Attr("disabled", true));
            document.Add(new TextboxTag("hidden", "something").Style("display", "none"));
        }

        private EnterValueGrammar grammarForName(string name)
        {
            var def = new GestureConfig{
                Template = "enter {data} for textbox",
                Finder = () => theDriver.FindElement(By.Name(name)),
                FinderDescription = "name=" + name,
                Description = "Enter data for blah",
                CellName = "data",
                
            };

            return new EnterValueGrammar(def);
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

            theDriver.FindElement(By.Name("emptytextbox")).GetAttribute("value").ShouldEqual("some text");
        }

        [Test]
        public void enter_text_for_a_textbox_that_is_not_initially_empty()
        {
            grammarForName("textbox").Execute(new Step().With("data", "different"));
            theDriver.FindElement(By.Name("textbox")).GetAttribute("value").ShouldEqual("different");
        }
    }
}