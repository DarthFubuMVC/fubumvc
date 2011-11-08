using System;
using OpenQA.Selenium;

namespace Serenity.Jasmine
{
    public enum JasmineMode
    {
        interactive,
        run
    }

    public class JasmineInput
    {
        public JasmineInput()
        {
            PortFlag = 5500;
            BrowserFlag = BrowserType.Chrome;

        }

        public JasmineMode Mode { get; set; }

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