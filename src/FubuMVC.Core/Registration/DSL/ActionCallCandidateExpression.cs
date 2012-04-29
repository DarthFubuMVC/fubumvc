using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Registration.DSL
{
    public class ActionCallCandidateExpression
    {
        private readonly ActionMethodFilter _methodFilter;
        private readonly ConfigurationGraph _configuration;
        private readonly Lazy<ActionSource> _mainSource;

        public ActionCallCandidateExpression(ActionMethodFilter methodFilter, ConfigurationGraph configuration)
        {
            _methodFilter = methodFilter;
            _configuration = configuration;
            _mainSource = new Lazy<ActionSource>(() =>
            {
                var source = new ActionSource(_methodFilter);
                configuration.AddActions(source);

                return source;
            });
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
            _mainSource.Value.TypeFilters.Excludes += filter;
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
            _mainSource.Value.IncludeTypes(filter);
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
        public ActionCallCandidateExpression IncludeMethods(Expression<Func<MethodInfo, bool>> filter)
        {
            _methodFilter.Includes += filter;
            return this;
        }

        /// <summary>
        /// Actions that match on the provided filter will NOT be added to the runtime. 
        /// </summary>
        public ActionCallCandidateExpression ExcludeMethods(Expression<Func<MethodInfo, bool>> filter)
        {
            _methodFilter.Excludes += filter;
            return this;
        }

        public ActionCallCandidateExpression IgnoreMethodsDeclaredBy<T>()
        {
            _methodFilter.IgnoreMethodsDeclaredBy<T>();
            return this;
        }

        public ActionCallCandidateExpression ForTypesOf<T>(Action<TypeMethodPolicy<T>> configure)
        {
            _mainSource.Value.TypeFilters.Includes += type => type.IsConcreteTypeOf<T>();

            var filter = new CompositeFilter<MethodInfo>();

            var policy = new TypeMethodPolicy<T>(filter);
            configure(policy);

            _mainSource.Value.CallFilters.Excludes +=
                call => call.HandlerType.IsConcreteTypeOf<T>() && !filter.Matches(call.Method);

            return this;
        }


        public ActionCallCandidateExpression ExcludeNonConcreteTypes()
        {
            _mainSource.Value.TypeFilters.Excludes += type => !type.IsConcrete();
            return this;
        }

        /// <summary>
        /// Find actions on the specified type
        /// </summary>
        public ActionCallCandidateExpression IncludeType<T>()
        {
            return FindWith(new SingleTypeActionSource(typeof (T), _methodFilter));
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
            _configuration.AddActions(actionSource);
            return this;
        }
    }
}