using OpenQA.Selenium;

namespace Serenity.WebDriver
{
    public static class JavaScriptExtensions
    {
        public static dynamic ToJS(this string str)
        {
            return JavaScript.Create(str);
        }

        public static dynamic ToJQueryJS(this string str)
        {
            return JavaScript.CreateWithJQueryCheck(str);
        }

        public static dynamic ToJQueryBy(this IWebElement element)
        {
            return JavaScript.JQueryFrom(element);
        }
    }
}