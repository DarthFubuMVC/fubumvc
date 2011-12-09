using System;
using System.Threading;
using FubuCore;
using FubuTestingSupport;
using HtmlTags;
using HtmlTags.Extended.Attributes;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures.Handlers;
using Serenity.Testing.Fixtures.Grammars;

namespace Serenity.Testing.Fixtures.Handlers
{
    [TestFixture]
    public class SelectElementHandlerTester
    {
        private IWebDriver theDriver;
        private IWebElement select1;
        private IWebElement nothingSelectedElement;
        private IWebElement select3;
        private SelectElementHandler theHandler = new SelectElementHandler();
        private IWebElement select4;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var document = new HtmlDocument();
            document.Add(new SelectTag(tag =>
            {
                tag.Option("a", "a");
                tag.Option("b", "2").Id("b");
                tag.Option("c", 3).Id("c");

                tag.SelectByValue("2");
            }).Name("select1").Id("select1"));

            document.Add(new SelectTag(tag =>
            {
                tag.Option("a", "a");
                tag.Option("b", 2).Id("b");
                tag.Option("c", 3).Id("c");

            }).Name("select2").Id("select2"));

            document.Add(new SelectTag(tag =>
            {
                tag.Add("option").Text("a");
                tag.Add("option").Text("b").Attr("selected", "selected");
                tag.Add("option").Text("c");

            }).Name("select3").Id("select3"));

            document.Add(new SelectTag(tag =>
            {
                tag.Option("a", "a");
                tag.Option("b", 2).Id("b");
                tag.Option("c", 3).Id("c");
                tag.Option("value", "b").Id("c");

                tag.SelectByValue(2);
            }).Name("select4").Id("select4"));
        
            document.WriteToFile("select.htm");

            try
            {
                startDriver();
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                startDriver();
            }

            select1 = theDriver.FindElement(By.Id("select1"));
            nothingSelectedElement = theDriver.FindElement(By.Id("select2"));
            select3 = theDriver.FindElement(By.Id("select3"));
            select4 = theDriver.FindElement(By.Id("select4"));
        }

        private void startDriver()
        {
            theDriver = WebDriverSettings.DriverBuilder()();
            theDriver.Navigate().GoToUrl("file:///" + "select.htm".ToFullPath());
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            theDriver.SafeDispose();
        }

        [Test]
        public void get_data_with_selected_option_with_both_value_and_text()
        {
            theHandler.GetData(null, select1).ShouldEqual("b=2");
        }

        [Test]
        public void get_data_with_selected_option_that_has_only_text()
        {
            theHandler.GetData(null, select3).ShouldEqual("b=b");
        }

        [Test]
        public void match_against_value_is_default()
        {
            theHandler.MatchesData(select1, 2).ShouldBeTrue();
            theHandler.MatchesData(select1, 3).ShouldBeFalse();
        }

        [Test]
        public void can_fall_thru_to_checking_against_display_if_value_does_not_exist()
        {
            theHandler.MatchesData(select1, "b").ShouldBeTrue();
            theHandler.MatchesData(select1, "c").ShouldBeFalse();
        }

        [Test]
        public void match_by_display_if_value_does_not_exist()
        {
            theHandler.MatchesData(select1, "b").ShouldBeTrue();
            theHandler.MatchesData(select1, "c").ShouldBeFalse();
        }

        [Test]
        public void does_not_fall_thru_to_display_if_the_expected_value_is_a_value()
        {
            theHandler.MatchesData(select4, "b").ShouldBeFalse();
        }

        [Test]
        public void matches_has_to_be_false_when_nothing_is_selected()
        {
            theHandler.MatchesData(nothingSelectedElement, "anything").ShouldBeFalse();
        }
    }
}