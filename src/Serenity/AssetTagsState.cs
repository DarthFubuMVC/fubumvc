using System.Collections.Generic;
using OpenQA.Selenium;

namespace Serenity
{
    public class AssetTagsState
    {
        public IList<IWebElement> Scripts { get; set; }
        public IList<IWebElement> Styles { get; set; }
    }
}