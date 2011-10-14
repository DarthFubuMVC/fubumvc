

using System.Diagnostics;
using System.Reflection;
using FubuTestingSupport;
using NUnit.Framework;
using System.Collections.Generic;
using OpenQA.Selenium;
using Serenity;

namespace IntegrationTesting
{
    [TestFixture, Explicit]
    public class debugger
    {
        [Test]
        public void try_to_set_up_the_environment()
        {
            var system = new FubuSystem();
            system.SetupEnvironment();
        }

        [Test]
        public void try_to_load_the_serenity_assembly()
        {
            var assembly = Assembly.Load("Serenity");
            assembly.GetExportedTypes().Each(x => Debug.WriteLine(x.FullName));
        }

        [Test]
        public void can_do_something_with_web_driver()
        {
            var settings = new WebDriverSettings()
            {
                Browser = BrowserType.Firefox
            };

            using (var browser = settings.DriverBuilder()())
            {
                browser.Navigate().GoToUrl("http://cnn.com");
                var head = browser.FindElement(By.TagName("head"));
                head.TagName.ShouldEqual("head");


                browser.Close();
            }
        }

        [Test]
        public void try_to_load_asset_fixture()
        {
            var assembly = Assembly.Load("WebDriver");
            assembly.GetExportedTypes().Each(x => Debug.WriteLine(x.FullName));
        }
    }
}