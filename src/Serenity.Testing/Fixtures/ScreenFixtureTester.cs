using System;
using System.Globalization;
using System.Threading;
using FubuLocalization;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using Serenity.Fixtures.Handlers;
using StoryTeller;
using System.Linq;
using FubuTestingSupport;
using HtmlTags.Extended.Attributes;
using StoryTeller.Domain;

namespace Serenity.Testing.Fixtures
{
    [TestFixture]
    public class ScreenFixtureIntegratedTester : ScreenManipulationTester
    {
        private IGrammar theGrammar;

        protected override void configureDocument(HtmlDocument document)
        {
            document.Add("input").Attr("type", "text").Name("Direction");

        }

        [Test]
        public void exercise_enter_value_grammar()
        {
            var fixture = new ScreenFixture<ViewModel>();
            fixture.PushElementContext(theDriver);

            theGrammar = fixture.EnterScreenValue(x => x.Direction);

            theGrammar.Execute(new Step().With("Direction", "North"));

            theDriver.FindElement(By.Name("Direction"))
                .GetAttribute("value").ShouldEqual("North");
        }

        [Test]
        public void exercise_enter_value_grammar_with_overriden_key()
        {
            var fixture = new ScreenFixture<ViewModel>();
            fixture.PushElementContext(theDriver);

            theGrammar = fixture.EnterScreenValue(x => x.Direction, key:"dir");

            theGrammar.Execute(new Step().With("dir", "North"));

            theDriver.FindElement(By.Name("Direction"))
                .GetAttribute("value").ShouldEqual("North");
        }

        [Test]
        public void exercise_check_value_grammar()
        {
            new TextboxElementHandler().EnterData(theDriver.FindElement(By.Name("Direction")), "South");

            var fixture = new ScreenFixture<ViewModel>();
            fixture.PushElementContext(theDriver);
            var grammar = fixture.CheckScreenValue(x => x.Direction);

            grammar.Execute(new Step().With("Direction", "South"))
                .Counts.ShouldEqual(1, 0, 0, 0);

            grammar.Execute(new Step().With("Direction", "East"))
                .Counts.ShouldEqual(0, 1, 0, 0);

        }
    }


    [TestFixture]
    public class ScreenFixtureTester
    {

    }

    [TestFixture]
    public class when_creating_a_check_grammar_with_defaults
    {
        private LineGrammar theGrammar;

        [SetUp]
        public void SetUp()
        {
            // Leave this here!
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            LocalizationManager.Stub();

            theGrammar = new ScreenFixture<ViewModel>().CheckScreenValue(x => x.Direction) as LineGrammar;
        }

        [Test]
        public void the_cell_value_should_be_named_for_the_property()
        {
            theGrammar.GetCells().Single().Key.ShouldEqual("Direction");
        }

        [Test]
        public void the_template_is_derived_from_the_property()
        {
            theGrammar.Template.ShouldEqual("The text of en-US_Direction should be {Direction}");
        }

        [Test]
        public void should_add_a_description()
        {
            theGrammar.Description.ShouldEqual("Check data for property Direction");
        }
    }

    [TestFixture]
    public class when_creating_an_enter_value_grammar_with_defaults
    {
        private LineGrammar theGrammar;

        [SetUp]
        public void SetUp()
        {
            // Leave this here!
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            LocalizationManager.Stub();
            
            theGrammar = new ScreenFixture<ViewModel>().EnterScreenValue(x => x.Direction) as LineGrammar;
        }

        [Test]
        public void the_cell_value_should_be_named_for_the_property()
        {
            theGrammar.GetCells().Single().Key.ShouldEqual("Direction");
        }

        [Test]
        public void the_template_is_derived_from_the_property()
        {
            theGrammar.Template.ShouldEqual("Enter {Direction} for en-US_Direction");
        }

        [Test]
        public void should_add_a_description()
        {
            theGrammar.Description.ShouldEqual("Enter data for property Direction");
        }
    }

    public class ViewModel
    {
        public string Direction { get; set; }
    }
}