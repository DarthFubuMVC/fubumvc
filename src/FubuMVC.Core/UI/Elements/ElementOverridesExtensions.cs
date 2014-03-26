using FubuMVC.Core.Registration;

namespace FubuMVC.Core.UI.Elements
{
    public static class ElementOverridesExtensions
    {
         public static IAccessorRulesExpression InputBuilder<T>(
             this IAccessorRulesExpression expression, string profile = null) where T : IElementBuilder, new()
         {
             var @override = new ElementTagOverride<T>(ElementConstants.Editor, profile);
             return expression.Add(@override);
         }

         public static IAccessorRulesExpression DisplayBuilder<T>(
             this IAccessorRulesExpression expression, string profile = null) where T : IElementBuilder, new()
         {
             var @override = new ElementTagOverride<T>(ElementConstants.Display, profile);
             return expression.Add(@override);
         }

         public static IAccessorRulesExpression LabelBuilder<T>(
             this IAccessorRulesExpression expression, string profile = null) where T : IElementBuilder, new()
         {
             var @override = new ElementTagOverride<T>(ElementConstants.Label, profile);
             return expression.Add(@override);
         }
    }
}