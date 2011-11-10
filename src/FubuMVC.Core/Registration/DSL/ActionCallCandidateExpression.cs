using System;
using System.Collections.Generic;
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
        private readonly IList<IActionSource> _sources;
        
        public ActionCallCandidateExpression(BehaviorMatcher matcher, TypePool types, IList<IActionSource> sources)
        {
            _matcher = matcher;
            _types = types;
            _sources = sources;
        }

        /// <summary>
        /// Find Actions on classes that end on 'Controller'
        /// </summary>
        public ActionCallCandidateExpression IncludeClassesSuffixedWithController()
        {
            return IncludeTypes(x => x.Name.EndsWith("Controller"));
        }

        /// <summary>
        /// Exclude types that match on the provided filter for finding Actions
        /// </summary>
        public ActionCallCandidateExpression ExcludeTypes(Expression<Func<Type, bool>> filter)
        {
            _matcher.TypeFilters.Excludes += filter;
            return this;
        }

        /// <summary>
        /// Find Action on types that match on the provided filter based on their name
        /// </summary>
        public ActionCallCandidateExpression IncludeTypesNamed(Expression<Func<string, bool>> filter)
        {
            var typeParam = Expression.Parameter(typeof (Type), "type"); // type =>
            var nameProp = Expression.Property(typeParam, "Name");  // type.Name
            var invokeFilter = Expression.Invoke(filter, nameProp); // filter(type.Name)
            var lambda = Expression.Lambda<Func<Type, bool>>(invokeFilter, typeParam); // type => filter(type.Name)
            
            return IncludeTypes(lambda);
        }

        /// <summary>
        /// Find Actions on types that match on the provided filter
        /// </summary>
        public ActionCallCandidateExpression IncludeTypes(Expression<Func<Type, bool>> filter)
        {
            _matcher.TypeFilters.Includes += filter;
            return this;
        }

        /// <summary>
        /// Find Actions on concrete types assignable to T
        /// </summary>
        public ActionCallCandidateExpression IncludeTypesImplementing<T>()
        {
            return IncludeTypes(type => !type.IsOpenGeneric() && type.IsConcreteTypeOf<T>());
        }

        /// <summary>
        /// Actions that match on the provided filter will be added to the runtime. 
        /// </summary>
        public ActionCallCandidateExpression IncludeMethods(Expression<Func<ActionCall, bool>> filter)
        {
            _matcher.MethodFilters.Includes += filter;
            return this;
        }

        /// <summary>
        /// Actions that match on the provided filter will NOT be added to the runtime. 
        /// </summary>
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

        /// <summary>
        /// Find actions on the specified type
        /// </summary>
        public ActionCallCandidateExpression IncludeType<T>()
        {
            _types.AddType(typeof (T));
            _matcher.TypeFilters.Includes += type => type == typeof (T);
            return this;
        }

        /// <summary>
        /// Find actions through an <see cref="IActionSource"/> instance.
        /// </summary>
        public ActionCallCandidateExpression FindWith<T>() where T : IActionSource, new()
        {
            return FindWith(new T());
        }

        /// <summary>
        /// Find actions with the provided <see cref="IActionSource"/> instance.
        /// </summary>
        public ActionCallCandidateExpression FindWith(IActionSource actionSource)
        {
            _sources.Fill(actionSource);
            return this;
        }
    }
}