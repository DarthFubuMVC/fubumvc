using System;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Tags;
using FubuValidation;
using FubuValidation.Registration;
using FubuValidation.Strategies;
using HtmlTags;

namespace FubuMVC.Validation
{
    public static class HtmlConventionExtensions
    {
        public static void ModifyForRule<TRule>(this TagFactoryExpression expression, Action<HtmlTag, TRule, Accessor> modification)
            where TRule : class, IValidationRule
        {
            expression
                .Always
                .Modify((request, tag) =>
                            {
                                var query = request.Get<IValidationQuery>();
                                var rule = query.GetRule<TRule>(request.Accessor);
                                if (rule == null) return;
                                modification(tag, rule, request.Accessor);
                            });
        }

        public static void ModifyForStrategy<TStrategy>(this TagFactoryExpression expression, Action<HtmlTag> modification)
            where TStrategy : class, IFieldValidationStrategy
        {
            expression.ModifyForStrategy<TStrategy>((tag, strategy, accessor) => modification(tag));
        }

        public static void ModifyForStrategy<TStrategy>(this TagFactoryExpression expression, Action<HtmlTag, TStrategy> modification)
            where TStrategy : class, IFieldValidationStrategy
        {
            expression.ModifyForStrategy<TStrategy>((tag, strategy, accessor) => modification(tag, strategy));
        }

        public static void ModifyForStrategy<TStrategy>(this TagFactoryExpression expression, Action<HtmlTag, TStrategy, Accessor> modification)
            where TStrategy : class, IFieldValidationStrategy
        {
            expression
                .Always
                .Modify((request, tag) =>
                            {
                                var query = request.Get<IValidationQuery>();
                                var strategy = query.GetStrategy<TStrategy>(request.Accessor);
                                if (strategy == null) return;
                                modification(tag, strategy, request.Accessor);
                            });
        }
    }
}