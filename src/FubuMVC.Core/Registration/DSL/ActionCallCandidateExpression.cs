using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.DSL
{
    public class ActionCallCandidateExpression
    {
        private readonly BehaviorMatcher _matcher;
        private readonly TypePool _types;
        private readonly ActionSourceMatcher _actionSourceMatcher;
        public ActionCallCandidateExpression(BehaviorMatcher matcher, TypePool types, ActionSourceMatcher actionSourceMatcher)
        {
            _matcher = matcher;
            _actionSourceMatcher = actionSourceMatcher;
            _types = types;
        }

        // more to come...


        public ActionCallCandidateExpression IncludeClassesSuffixedWithController()
        {
            return IncludeTypes(x => x.Name.EndsWith("Controller"));
        }


        // TODO -- something in here that can close on types?

        public ActionCallCandidateExpression ExcludeTypes(Expression<Func<Type, bool>> filter)
        {
            _matcher.TypeFilters.Excludes += filter;
            return this;
        }

        public ActionCallCandidateExpression IncludeTypesNamed(Expression<Func<string, bool>> filter)
        {
            var typeParam = Expression.Parameter(typeof (Type), "type"); // type =>
            var nameProp = Expression.Property(typeParam, "Name");  // type.Name
            var invokeFilter = Expression.Invoke(filter, nameProp); // filter(type.Name)
            var lambda = Expression.Lambda<Func<Type, bool>>(invokeFilter, typeParam); // type => filter(type.Name)
            
            return IncludeTypes(lambda);
        }

        public ActionCallCandidateExpression IncludeTypes(Expression<Func<Type, bool>> filter)
        {
            _matcher.TypeFilters.Includes += filter;
            return this;
        }

        public ActionCallCandidateExpression IncludeTypesImplementing<T>()
        {
            return IncludeTypes(type => !type.IsOpenGeneric() && type.IsConcreteTypeOf<T>());
        }

        public ActionCallCandidateExpression IncludeMethods(Expression<Func<ActionCall, bool>> filter)
        {
            _matcher.MethodFilters.Includes += filter;
            return this;
        }

        public ActionCallCandidateExpression ExcludeMethods(Expression<Func<ActionCall, bool>> filter)
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

        public ActionCallCandidateExpression FindWith<T>() where T : IActionSource, new()
        {
            return FindWith(new T());
        }

        public ActionCallCandidateExpression FindWith(IActionSource actionSource)
        {
            _actionSourceMatcher.AddSource(actionSource);
            return this;
        }
    }
}