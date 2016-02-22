namespace FubuMVC.Core.Validation
{
    public static class TemplateExtensions
    {
        public static string AsTemplateField(this string value)
        {
            return "{" + value + "}";
        }
    }
}