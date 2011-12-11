using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using OpenQA.Selenium;

namespace Serenity
{
    public static class WebElementExtensions
    {
        public static bool IsCssLink(this IWebElement element)
        {
            return element.TagName == "link" &&
                   element.GetMimeType() == MimeType.Css;
        }

        public static string Href(this IWebElement element)
        {
            return element.GetAttribute("href");
        }

        public static string AssetName(this IWebElement element)
        {
            var parts = (element.Href() ?? element.GetAttribute("src")).Split('/').ToList();
            var index = parts.IndexOf(UrlRegistry.AssetsUrlFolder);

            return parts.Skip(index).Join("/");
        }

        public static MimeType GetMimeType(this IWebElement element)
        {
            return MimeType.MimeTypeByFileName(element.Href());
        }

        public static bool IsHiddenInput(this IWebElement element)
        {
            return element.TagName == "input" && element.GetAttribute("type") == "hidden";
        }
    }
}