using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.UI;
using FubuMVC.UI.Tags;
using FubuValidation.Strategies;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuValidation
{
    public interface IValidationQuery
    {
        T GetRule<T>(Accessor accessor) where T : class, IValidationRule;
        T GetStrategy<T>(Accessor accessor) where T : class, IFieldValidationStrategy;
        void ForRule<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IValidationRule;
        void ForStrategy<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IFieldValidationStrategy;
        bool HasRule<T>(Accessor accessor) where T : IValidationRule;
        bool HasStrategy<T>(Accessor accessor) where T : IFieldValidationStrategy;
    }

    public class ValidationQuery : IValidationQuery
    {
        private readonly ITypeResolver _typeResolver;
        private readonly IEnumerable<IValidationSource> _sources;

        public ValidationQuery(ITypeResolver typeResolver, IEnumerable<IValidationSource> sources)
        {
            _typeResolver = typeResolver;
            _sources = sources;
        }

        public T GetRule<T>(Accessor accessor) where T : class, IValidationRule
        {
            foreach (var source in _sources)
            {
                var targetRule = source
                                    .RulesFor(accessor.OwnerType)
                                    .FirstOrDefault(rule => typeof (T) == _typeResolver.ResolveType(rule));

                if(targetRule != null)
                {
                    return null;
                }
            }

            return null;
        }

        public T GetStrategy<T>(Accessor accessor) 
            where T : class, IFieldValidationStrategy
        {
            foreach (var source in _sources)
            {
                var targetRule = source
                                    .RulesFor(accessor.OwnerType)
                                    .FirstOrDefault(rule => typeof(FieldRule) == rule.GetType())
                                    .As<FieldRule>();

                if (targetRule != null && typeof(IFieldValidationStrategy) == _typeResolver.ResolveType(targetRule.Strategy))
                {
                    return targetRule
                            .Strategy
                            .As<T>();
                }
            }

            return null;
        }

        public void ForRule<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IValidationRule
        {
            var rule = GetRule<T>(accessor);
            if(rule != null)
            {
                action(rule, accessor);
            }
        }

        public void ForStrategy<T>(Accessor accessor, Action<T, Accessor> action) where T : class, IFieldValidationStrategy
        {
            var strategy = GetStrategy<T>(accessor);
            if (strategy != null)
            {
                action(strategy, accessor);
            }
        }

        public bool HasRule<T>(Accessor accessor) where T : IValidationRule
        {
            foreach (var source in _sources)
            {
                if(source.RulesFor(accessor.OwnerType).Any(rule => typeof (T) == _typeResolver.ResolveType(rule)))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStrategy<T>(Accessor accessor) where T : IFieldValidationStrategy
        {
            foreach (var source in _sources)
            {
                var targetRule = source
                                    .RulesFor(accessor.OwnerType)
                                    .FirstOrDefault(rule => typeof(FieldRule) == rule.GetType())
                                    .As<FieldRule>();

                if (targetRule != null && typeof(IFieldValidationStrategy) == _typeResolver.ResolveType(targetRule.Strategy))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class ValidationQueryExtensions
    {
        public static bool IsRequired(this IValidationQuery query, Accessor accessor)
        {
            return query.HasStrategy<RequiredFieldStrategy>(accessor);
        }
    }

    public class Blah : HtmlConventionRegistry
    {
        public Blah()
        {
            Editors.ModifyForStrategy<RequiredFieldStrategy>((tag, strategy, accessor) => tag.AddClass("required"));
            Editors.ModifyForStrategy<MaximumStringLengthFieldStrategy>((tag, strategy, accessor) => tag.Attr("maxlength", strategy.Length));
        }
    }

    public static class Test
    {
        public static void ModifyForRule<RULE>(this TagFactoryExpression expression, Action<HtmlTag, RULE, Accessor> modification) 
            where RULE : class, IValidationRule
        {
            // ew?
            var query = ServiceLocator.Current.GetInstance<IValidationQuery>();
            expression
                .If(def => query.HasRule<RULE>(def.Accessor))
                .Modify((request, tag) =>
                            {
                                var rule = query.GetRule<RULE>(request.Accessor);
                                modification(tag, rule, request.Accessor);
                            });
        }

        public static void ModifyForStrategy<STRATEGY>(this TagFactoryExpression expression, Action<HtmlTag, STRATEGY, Accessor> modification)
            where STRATEGY : class, IFieldValidationStrategy
        {
            // ew?
            var query = ServiceLocator.Current.GetInstance<IValidationQuery>();
            expression
                .If(def => query.HasStrategy<STRATEGY>(def.Accessor))
                .Modify((request, tag) =>
                {
                    var strategy = query.GetStrategy<STRATEGY>(request.Accessor);
                    modification(tag, strategy, request.Accessor);
                });
        }
    }
}