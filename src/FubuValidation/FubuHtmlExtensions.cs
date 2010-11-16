using System;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Tags;
using FubuValidation.Registration;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuValidation
{
    public static class FubuHtmlExtensions
    {
        public static void ModifyForRule<TRule>(this TagFactoryExpression expression, Action<HtmlTag, TRule, Accessor> modification)
            where TRule : class, IValidationRule
        {
            var query = ServiceLocator.Current.GetInstance<IValidationQuery>();
            expression
                .If(def => query.HasRule<TRule>(def.Accessor))
                .Modify((request, tag) =>
                            {
                                var rule = query.GetRule<TRule>(request.Accessor);
                                modification(tag, rule, request.Accessor);
                            });
        }

        public static void ModifyForStrategy<TStrategy>(this TagFactoryExpression expression, Action<HtmlTag, TStrategy, Accessor> modification)
            where TStrategy : class, IFieldValidationStrategy
        {
            var query = ServiceLocator.Current.GetInstance<IValidationQuery>();
            expression
                .If(def => query.HasStrategy<TStrategy>(def.Accessor))
                .Modify((request, tag) =>
                {
                    var strategy = query.GetStrategy<TStrategy>(request.Accessor);
                    modification(tag, strategy, request.Accessor);
                });
        }
    }
}