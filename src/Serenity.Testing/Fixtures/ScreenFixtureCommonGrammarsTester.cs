using System;
using System.Threading;
using FubuCore;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using StoryTeller;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using StoryTeller.Engine;
using TestContext = StoryTeller.Engine.TestContext;

namespace Serenity.Testing.Fixtures
{
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
            return new ClickGrammar("Clicking " + id, () => theDriver.FindElement(By.Id(id))){
                FinderDescription="#" + id
            };
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
            grammar.FinderDescription.IsNotEmpty().ShouldBeTrue();

            Exception<StorytellerAssertionException>.ShouldBeThrownBy(() =>
            {
                grammar.Execute();
            }).ShouldContainErrorMessage(grammar.FinderDescription);
        }

        [Test]
        public void check_click_grammar_happy_path()
        {
            var grammar = new ClickGrammar(
                "the click grammar",
                () => theDriver.FindElement(By.Id("happyPath"))){
                };

            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);

            theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");
        }

        [Test]
        public void calls_the_action_after_a_successful_click()
        {
            Action action = () => theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");

            var grammar = new ClickGrammar(
                "the click grammar",
                () => theDriver.FindElement(By.Id("happyPath"))){
                    AfterClick = action
                };

            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);
        }
        
        [Test]
        public void before_click_is_called_before_trying_to_click()
        {
            var grammar = new ClickGrammar(
                "the click grammar",
                () => theDriver.FindElement(By.Id("happyPath")));

            var wasCalled = false;
            grammar.BeforeClick = () =>
            {
                wasCalled = true;
                theDriver.FindElement(By.Id("clickTarget")).Text.ShouldBeEmpty();
            };

            grammar.Execute();

            wasCalled.ShouldBeTrue();
        }
    }


    public abstract class ScreenManipulationTester
    {
        [SetUp]
        public void SetUp()
        {
            new FileSystem().DeleteFile("screenfixture.htm");

            var document = new HtmlDocument();
            configureDocument(document);

            document.WriteToFile("screenfixture.htm");

            theDriver = WebDriverSettings.DriverBuilder()();
            theDriver.Navigate().GoToUrl("file:///" + "screenfixture.htm".ToFullPath());
            theFixture = new StubScreenFixture(theDriver);
        }

        [TearDown]
        public void TearDown()
        {
            theDriver.SafeDispose();
        }

        protected IWebDriver theDriver;
        protected StubScreenFixture theFixture;

        protected abstract void configureDocument(HtmlDocument document);
    }



    public class StubScreenFixture : ScreenFixture
    {
        public StubScreenFixture(IWebDriver driver)
        {
            pushSearchContext(driver);
        }
    }

    public class StepExecutionResult
    {
        public IStepResults Results { get; set; }
        public Counts Counts { get; set; }
    }

    public static class TestingExtensions
    {
        public static StepExecutionResult Execute(this IGrammar grammar, IStep step)
        {
            var context = new TestContext();

            grammar.Execute(step, context);

            return new StepExecutionResult{
                Counts = context.Counts,
                Results = context.ResultsFor(step)
            };
        }

        public static StepExecutionResult Execute(this IGrammar grammar)
        {
            return grammar.Execute(new Step());
        }

        public static void ShouldEqual(this Counts counts, int rights, int wrongs, int exceptions, int syntaxErrors)
        {
            var expected = new Counts
                           {
                               Rights = rights,
                               Wrongs = wrongs,
                               Exceptions = exceptions,
                               SyntaxErrors = syntaxErrors
                           };

            Assert.AreEqual(expected, counts);
        }
    }
}