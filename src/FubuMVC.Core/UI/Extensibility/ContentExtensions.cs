using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Extensibility
{
    public static class ContentExtensions
    {
        public static void WriteExtensions<T>(this IFubuPage<T> page) where T : class
        {
            page.Get<ContentExtensionGraph>().ApplyExtensions(page);
        }

        public static void WriteExtensions<T>(this IFubuPage<T> page, string tag) where T : class
        {
            page.Get<ContentExtensionGraph>().ApplyExtensions(page, tag);
        }

        public static ExtensionsExpression Extensions(this FubuRegistry registry)
        {
            return new ExtensionsExpression(registry);
        }
    }
}