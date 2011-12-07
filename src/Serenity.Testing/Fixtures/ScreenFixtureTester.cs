using System;
using System.Globalization;
using System.Threading;
using FubuLocalization;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using Serenity.Fixtures.Grammars;
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
            document.Add("input").Attr("type", "text").Name("Blank")
                .Attr("onclick", "document.getElementById('clickTarget').innerHTML = 'clicked'");

            document.Add("div").Id("clickTarget");
            document.Add("button").Id("happyPath").Text("Hey there!").Attr("onclick", "document.getElementById('clickTarget').innerHTML = 'clicked'");

        }



        private IGrammar grammarNamed(string name)
        {
            var fixture = new FakeFixture(theDriver);
            return fixture[name];
        }

        [Test]
        public void exercise_click_by_specific_selector()
        {
            var grammar = grammarNamed("selector");
            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);

            theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");
        }

        [Test]
        public void exercise_click_by_id()
        {
            var grammar = grammarNamed("id");
            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);

            theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");
        }

        [Test]
        public void exercise_click_by_name()
        {
            var grammar = grammarNamed("name");
            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);

            theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");
        }

        [Test]
        public void exercise_click_by_css()
        {
            var grammar = grammarNamed("css");
            grammar.Execute().Counts.ShouldEqual(0, 0, 0, 0);

            theDriver.FindElement(By.Id("clickTarget")).Text.ShouldEqual("clicked");
        }




        [Test]
        public void exercise_enter_value_grammar()
        {
            var fixture = new ScreenFixture<ViewModel>();

            theGrammar = grammarNamed("EnterDirection");

            theGrammar.Execute(new Step().With("Direction", "North"));

            theDriver.FindElement(By.Name("Direction"))
                .GetAttribute("value").ShouldEqual("North");
        }

        [Test]
        public void exercise_enter_value_grammar_with_overriden_key()
        {
            theGrammar = grammarNamed("EnterDirection2");

            theGrammar.Execute(new Step().With("dir", "North"));

            theDriver.FindElement(By.Name("Direction"))
                .GetAttribute("value").ShouldEqual("North");
        }

        public class FakeFixture : ScreenFixture<ViewModel>
        {

            public FakeFixture(ISearchContext context)
            {
                PushElementContext(context);

                this["selector"] = Click(selector: By.Id("happyPath"));
                this["css"] = Click(css: "#happyPath");
                this["id"] = Click(id: "happyPath");
                this["name"] = Click(name: "Blank");

                this["EnterDirection"] = EnterScreenValue(x => x.Direction);
                this["EnterDirection2"] = EnterScreenValue(x => x.Direction, key: "dir");
                this["CheckDirection"] = CheckScreenValue(x => x.Direction);
            }
        }

        [Test]
        public void exercise_check_value_grammar()
        {
            new TextboxElementHandler().EnterData(theDriver.FindElement(By.Name("Direction")), "South");

            var grammar = grammarNamed("CheckDirection");

            grammar.Execute(new Step().With("Direction", "South"))
                .Counts.ShouldEqual(1, 0, 0, 0);

            grammar.Execute(new Step().With("Direction", "East"))
                .Counts.ShouldEqual(0, 1, 0, 0);

        }
    }


    [TestFixture]
    public class ScreenFixtureTester
    {
        private TheFixture theFixture;

        public class TheFixture : ScreenFixture<ViewModel>
        {
            public TheFixture()
            {
                EditableElement(x => x.Direction);
            }
        }

        [SetUp]
        public void SetUp()
        {
            theFixture = new TheFixture();
        }

        [Test]
        public void editable_element_adds_check_value_grammar()
        {
            theFixture["CheckDirection"].ShouldBeOfType<CheckValueGrammar>();
        }

        [Test]
        public void editable_element_adds_enter_value_grammar()
        {
            theFixture["CheckDirection"].ShouldBeOfType<CheckValueGrammar>();
        }
    }

    [TestFixture]
    public class when_creating_a_click_grammar
    {
        public class ClickFixture : ScreenFixture
        {
            public ClickFixture()
            {
                this["click1"] = Click(css: ".big").As<ClickGrammar>();
                this["click2"] = Click(css: ".big", label: "the big thing");
                this["click3"] = Click(css: ".big", template: "launch the something");
            }
        }

        [Test]
        public void by_css_no_label_no_template()
        {
            var grammar = new ClickFixture()["click1"].As<ClickGrammar>();

            grammar.Template.ShouldEqual("Click CssSelector: .big");
        }

        [Test]
        public void by_css_with_a_label_but_no_template()
        {
            var grammar = new ClickFixture()["click2"].As<ClickGrammar>();

            grammar.Template.ShouldEqual("Click the big thing");
        }

        [Test]
        public void by_css_with_a_template()
        {
            var fixture = new ScreenFixture<ViewModel>();
            var grammar = new ClickFixture()["click3"].As<ClickGrammar>();

            grammar.Template.ShouldEqual("launch the something");
        }
    }

    [TestFixture]
    public class when_creating_a_check_grammar_with_defaults
    {
        private LineGrammar theGrammar;

        public class CheckGrammarFixture : ScreenFixture<ViewModel>
        {
            public CheckGrammarFixture()
            {
                this["CheckDirection"] = CheckScreenValue(x => x.Direction);
            }
        }

        [SetUp]
        public void SetUp()
        {
            // Leave this here!
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            LocalizationManager.Stub();

            theGrammar = new CheckGrammarFixture()["CheckDirection"] as LineGrammar;
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

        public class EnterGrammarFixture : ScreenFixture<ViewModel>
        {
            public EnterGrammarFixture()
            {
                this["EnterDirection"] = EnterScreenValue(x => x.Direction);
            }
        }

        [SetUp]
        public void SetUp()
        {
            // Leave this here!
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            LocalizationManager.Stub();

            theGrammar = new EnterGrammarFixture()["EnterDirection"] as LineGrammar;
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