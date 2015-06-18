using System;
using System.Linq;
using OpenQA.Selenium;

namespace Serenity
{
    public class ScreenDriver
    {
        private readonly IWebDriver _browser;
        private readonly Lazy<IWebElement> _head;

        public ScreenDriver(IWebDriver browser)
        {
            _browser = browser;
            _head = new Lazy<IWebElement>(() => _browser.FindElement(By.TagName("head")));
        }

        private IWebElement head
        {
            get { return _head.Value; }
        }

        private IWebElement elementFor(string name)
        {
            return _browser.FindElement(By.Id(name)) ?? _browser.FindElement(By.Name(name));
        }

        public AssetTagsState GetAssetDeclarationsFromTheHead()
        {
            return new AssetTagsState
            {
                Scripts = head.FindElements(By.TagName("script")).ToList(),
                Styles = head.FindElements(By.TagName("link")).Where(x => x.IsCssLink()).ToList()
            };
        }

        public AssetTagsState GetAssetDeclarations()
        {
            return new AssetTagsState
            {
                Scripts = _browser.FindElements(By.TagName("script")).ToList(),
                Styles = _browser.FindElements(By.TagName("link")).Where(x => x.IsCssLink()).ToList()
            };
        }
    }
}