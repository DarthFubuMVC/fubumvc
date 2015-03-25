using System;
using FubuCore.Util;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Serenity.Testing
{
    [SetUpFixture]
    public class BrowserForTesting
    {
        private static readonly Cache<Type, IBrowserLifecycle> Browsers = new Cache<Type, IBrowserLifecycle>();

        public static IWebDriver Driver { get; private set; }

        public static IBrowserLifecycle Use<TBrowser>() where TBrowser : IBrowserLifecycle, new()
        {
            var type = typeof (TBrowser);

            if (!Browsers.Has(type))
            {
                Browsers.Fill(type, key => new TBrowser());
            }

            var lifecycle = Browsers[type];

            Driver = lifecycle.Driver;
            return lifecycle;
        }

        [TearDown]
        public void Teardown()
        {
            Browsers.Each(x => x.Dispose());
            Browsers.ClearAll();
        }
    }
}