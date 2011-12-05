using System;
using FubuCore;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using StoryTeller.Assertions;

namespace Serenity.Testing.Fixtures
{
    [TestFixture]
    public class ClickGrammerTester : ScreenManipulationTester
    {
        protected override void configureDocument(HtmlDocument doc)
        {
            doc.Add("div").Id("clickTarget");
            doc.Add("button").Id("happyPath").Text("Hey there!").Attr("onclick", "document.getElementById('clickTarget').innerHTML = 'clicked'");

            doc.Add("button").Id("disabled").Text("disabled").Attr("disabled", true);

            doc.Add("button").Id("hidden").Text("hidden").Style("display", "none");
        }

        private ClickGrammar grammarForId(string id)
        {
            var config = new GestureConfig{
                Finder = () => theDriver.FindElement(By.Id(id)),
                FinderDescription = "#" + id,
                Template = "Clicking " + id
            };

            return new ClickGrammar(config);
        }

        [Test]
        public void click_element_that_does_not_exist_should_throw_descriptive_exception()
        {
            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammarForId("notexistent").Execute();
            }).ShouldContainErrorMessage(ClickGrammar.NonexistentElementMessage);
        }

        [Test]
        public void click_hidden_element_should_throw_exception()
        {
            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammarForId("hidden").Execute();
            }).ShouldContainErrorMessage(ClickGrammar.HiddenElementMessage);
        }

        [Test]
        public void click_disabled_element_should_throw_exception()
        {
            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammarForId("disabled").Execute();
            }).ShouldContainErrorMessage(ClickGrammar.DisabledElementMessage);
        }

        [Test]
        public void use_location_in_disabled_message_if_it_exists()
        {
            var grammar = grammarForId("disabled");
            grammar.Config.FinderDescription.IsNotEmpty().ShouldBeTrue();

            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammar.Execute();
            }).ShouldContainErrorMessage(grammar.Config.FinderDescription);
        }

        [Test]
        public void check_click_grammar_happy_path()
        {
            var grammar = grammarForId("happyPath");

            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);

            theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");
        }

        [Test]
        public void calls_the_action_after_a_successful_click()
        {
            Action action = () => theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");

            var grammar = grammarForId("happyPath");
            grammar.Config.AfterClick = action;

            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);
        }
        
        [Test]
        public void before_click_is_called_before_trying_to_click()
        {
            var grammar = grammarForId("happyPath");
            var wasCalled = false;
            grammar.Config.BeforeClick = () =>
            {
                wasCalled = true;
                theDriver.FindElement(By.Id("clickTarget")).Text.ShouldBeEmpty();
            };

            grammar.Execute();

            wasCalled.ShouldBeTrue();
        }
    }
}