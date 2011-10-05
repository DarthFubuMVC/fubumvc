using System;
using FubuMVC.Core;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using FubuCore;
using FubuTestingSupport;

namespace Serenity.Testing
{
    [TestFixture, Explicit]
    public class TryItOut : BrowserFixture
    {
        [Test]
        public void lets_just_see()
        {
            OpenTo(x => x.Add("div").Text("Hello").Id("hello"));

            driver.FindElement(By.Id("hello")).Text.ShouldEqual("Hello");


        }
    }

    [TestFixture]
    public abstract class BrowserFixture
    {
        private IWebDriver _driver;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _driver = new FirefoxDriver();
        }

        protected IWebDriver driver
        {
            get
            {
                return _driver;
            }
        }

        public void OpenTo(string file)
        {
            var path = file.ToFullPath();
            _driver.Navigate().GoToUrl("file:///" + path);
        }

        public void OpenTo(Action<HtmlDocument> configure)
        {
            var document = new HtmlDocument();
            configure(document);
            var fileName = "doc.htm";

            document.WriteToFile(fileName);

            OpenTo(fileName);
        }



        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            _driver.Close();
            _driver.SafeDispose();
        }
    }
}