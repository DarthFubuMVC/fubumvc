using System;
using System.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Registration.DSL
{
    public class ActionCallCandidateExpression
    {
        private readonly BehaviorMatcher _matcher;
        private readonly TypePool _types;

        public ActionCallCandidateExpression(BehaviorMatcher matcher, TypePool types)
        {
            _matcher = matcher;
            _types = types;
        }

        // more to come...

        public ActionCallCandidateExpression ExcludeTypes(Func<Type, bool> filter)
        {
            _matcher.TypeFilters.Excludes += filter;
            return this;
        }

        public ActionCallCandidateExpression IncludeTypesNamed(Func<string, bool> filter)
        {
            return IncludeTypes(type => filter(type.Name));
        }

        public ActionCallCandidateExpression IncludeTypes(Func<Type, bool> filter)
        {
            _matcher.TypeFilters.Includes += filter;
            return this;
        }

        public ActionCallCandidateExpression IncludeTypesImplementing<T>()
        {
            return IncludeTypes(type => !type.IsOpenGeneric() && type.IsConcreteTypeOf<T>());
        }

        public ActionCallCandidateExpression IncludeMethods(Func<ActionCall, bool> filter)
        {
            _matcher.MethodFilters.Includes += filter;
            return this;
        }

        public ActionCallCandidateExpression ExcludeMethods(Func<ActionCall, bool> filter)
        {
            _matcher.MethodFilters.Excludes += filter;
            return this;
        }

        public ActionCallCandidateExpression IgnoreMethodsDeclaredBy<T>()
        {
            return ExcludeMethods(call => call.Method.DeclaringType == typeof (T));
        }

        public ActionCallCandidateExpression ForTypesOf<T>(Action<TypeMethodPolicy<T>> configure)
        {
            _matcher.TypeFilters.Includes += type => type.IsConcreteTypeOf<T>();

            var filter = new CompositeFilter<MethodInfo>();

            var policy = new TypeMethodPolicy<T>(filter);
            configure(policy);

            _matcher.MethodFilters.Excludes +=
                call => call.HandlerType.IsConcreteTypeOf<T>() && !filter.Matches(call.Method);

            return this;
        }


        public ActionCallCandidateExpression ExcludeNonConcreteTypes()
        {
            _matcher.TypeFilters.Excludes += type => !type.IsConcrete();
            return this;
        }

        public ActionCallCandidateExpression IncludeType<T>()
        {
            _types.AddType(typeof (T));
            _matcher.TypeFilters.Includes += type => type == typeof (T);
            return this;
        }
    }
}