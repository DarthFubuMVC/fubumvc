using OpenQA.Selenium;

namespace Serenity.WebDriver
{
    public static class JavaScriptExtensionMethods
    {
        public static void Execute(this IWebDriver driver, JavaScript script)
        {
            script.Execute((IJavaScriptExecutor) driver);
        }

        public static object ExecuteAndGet(this IWebDriver driver, JavaScript script)
        {
            return script.ExecuteAndGet((IJavaScriptExecutor) driver);
        }

        public static T ExecuteAndGet<T>(this IWebDriver driver, JavaScript script) where T : class
        {
            return script.ExecuteAndGet<T>((IJavaScriptExecutor) driver);
        }
    }
}