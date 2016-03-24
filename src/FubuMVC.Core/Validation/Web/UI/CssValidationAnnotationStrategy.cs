using System;
using FubuCore.Util;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class CssValidationAnnotationStrategy : IValidationAnnotationStrategy
    {
        private static readonly Cache<Type, string> Classes = new Cache<Type, string>();

        static CssValidationAnnotationStrategy()
        {
            defineClass<RequiredFieldRule>("required");
            defineClass<GreaterThanZeroRule>("greater-than-zero");
            defineClass<GreaterOrEqualToZeroRule>("greater-equal-zero");
            defineClass<EmailFieldRule>("email");
        }

        private static void defineClass<T>(string css)
        {
            Classes[typeof (T)] = css;
        }

        public bool Matches(IFieldValidationRule rule)
        {
            return Classes.Has(rule.GetType());
        }

        public void Modify(ElementRequest request, IFieldValidationRule rule)
        {
            var tag = request.CurrentTag;
            tag.AddClass(Classes[rule.GetType()]);
        }
    }
}