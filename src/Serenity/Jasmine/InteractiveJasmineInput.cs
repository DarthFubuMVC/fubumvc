using System;
using OpenQA.Selenium;

namespace Serenity.Jasmine
{
    public class InteractiveJasmineInput
    {
        public InteractiveJasmineInput()
        {
            PortFlag = 5500;
            BrowserFlag = BrowserType.Chrome;

        }

        public string SerenityFile { get; set; }
        public int PortFlag { get; set; }
        public BrowserType BrowserFlag { get; set; }

        public Func<IWebDriver> GetBrowserBuilder()
        {
            return new WebDriverSettings{
                Browser = BrowserFlag
            }.DriverBuilder();
        }
    }
}