namespace Serenity.WebDriver
{
    public class By : OpenQA.Selenium.By
    {
        /// <summary>
        /// jQuery selector
        /// </summary>
        public static dynamic jQuery(string selector)
        {
            return JavaScript.CreateJQuery(selector);
        }
    }
}
