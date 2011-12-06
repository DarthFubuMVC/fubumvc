using HtmlTags;
using HtmlTags.Extended.Attributes;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures.Grammars;
using StoryTeller.Domain;

namespace Serenity.Testing.Fixtures.Grammars
{
    [TestFixture]
    public class CheckValueGrammarTester : ScreenManipulationTester
    {
        protected override void configureDocument(HtmlDocument document)
        {
            document.Add(new TextboxTag("emptytextbox", string.Empty).Id("emptytextbox"));
            document.Add(new TextboxTag("textbox", "initial").Id("textbox"));

            document.Add(new SelectTag(tag =>
            {
                tag.Option("a", "a");
                tag.Option("b", 2).Id("b");
                tag.Option("c", 3).Id("c");

                tag.SelectByValue("2");
            }).Name("select1").Id("select1"));

            document.Add("div").Id("div1").Text("the div text");
        }

        private CheckValueGrammar grammarForId(string id)
        {
            var config = new GestureConfig{
                Finder = () => theDriver.FindElement(By.Id(id)),
                FinderDescription = "#" + id,
                CellName = "data"
            };

            return new CheckValueGrammar(config);
        }

        [Test]
        public void positive_case_for_select_verifying_by_value()
        {
            var grammar = grammarForId("select1");
            var step = new Step().With("data", 2);

            grammar.Execute(step).Counts.ShouldEqual(1, 0, 0, 0);
        }

        [Test]
        public void positive_case_for_select_verifying_by_display()
        {
            var grammar = grammarForId("select1");
            var step = new Step().With("data", "b");

            grammar.Execute(step).Counts.ShouldEqual(1, 0, 0, 0);
        }

        [Test]
        public void negative_case_for_select()
        {
            var grammar = grammarForId("select1");
            var step = new Step().With("data", "different");
 
            grammar.Execute(step).Counts.ShouldEqual(0, 1, 0, 0);
        }

        [Test]
        public void positive_case_against_textbox()
        {
            var grammar = grammarForId("textbox");

            grammar.Execute(new Step().With("data", "initial"))
                .Counts.ShouldEqual(1, 0, 0, 0);
        }

        [Test]
        public void negative_case_against_textbox()
        {
            var grammar = grammarForId("textbox");

            grammar.Execute(new Step().With("data", "something different"))
                .Counts.ShouldEqual(0, 1, 0, 0);
        }

        [Test]
        public void positive_case_against_div()
        {
            var grammar = grammarForId("div1");
            var step = new Step().With("data", "the div text");

            grammar.Execute(step).Counts.ShouldEqual(1, 0, 0, 0);
        }

        [Test]
        public void negative_case_against_div()
        {
            var grammar = grammarForId("div1");
            var step = new Step().With("data", "different text");

            grammar.Execute(step).Counts.ShouldEqual(0, 1, 0, 0);
        }

        [Test]
        public void negative_case_with_syntax_error()
        {
            var grammar = grammarForId("div1");
            var step = new Step();

            grammar.Execute(step).Counts.ShouldEqual(0, 0, 0, 1);
        }


    }
}