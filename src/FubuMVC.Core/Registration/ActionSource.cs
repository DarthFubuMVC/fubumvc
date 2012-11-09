using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration
{
    public class ActionSource : IActionSource
    {
        private readonly IList<Action<TypePool>> _typePoolConfigurations = new List<Action<TypePool>>(); 
        private readonly AppliesToExpression _applies = new AppliesToExpression();

        public static bool IsCandidate(MethodInfo method)
        {
            if (method.DeclaringType.Equals(typeof(object))) return false;

            int parameterCount = method.GetParameters() == null ? 0 : method.GetParameters().Length;
            if (parameterCount > 1) return false;


            
            bool hasOutput = method.ReturnType != typeof (void);
            if (hasOutput && !method.ReturnType.IsClass) return false;

            if (hasOutput) return true;

            return parameterCount == 1;
        }

        private readonly ActionMethodFilter _methodFilters;
        private readonly CompositeFilter<Type> _typeFilters = new CompositeFilter<Type>();
        private readonly CompositeFilter<ActionCall> _callFilters = new CompositeFilter<ActionCall>();

        public ActionSource()
        {
            _methodFilters = new ActionMethodFilter();
        }

        public AppliesToExpression Applies
        {
            get { return _applies; }
        }


        IEnumerable<ActionCall> IActionSource.FindActions(Assembly applicationAssembly)
        {
            var types = _applies.BuildPool(applicationAssembly);

            return types.TypesMatching(_typeFilters.Matches).SelectMany(actionsFromType);
        }

        private IEnumerable<ActionCall> actionsFromType(Type type)
        {
            return type.PublicInstanceMethods()
                .Where(_methodFilters.Matches)
                .Select(m => buildAction(type, m))
                .Where(_callFilters.Matches);
        }

        protected virtual ActionCall buildAction(Type type, MethodInfo method)
        {
            return new ActionCall(type, method);
        }

        /// <summary>
        /// Find Actions on classes whose name ends on 'Controller'
        /// </summary>
        public void IncludeClassesSuffixedWithController()
        {
            IncludeTypesNamed(x => x.EndsWith("Controller"));
        }

        /// <summary>
        /// Find Actions on classes whose name ends with 'Endpoint'
        /// </summary>
        public void IncludeClassesSuffixedWithEndpoint()
        {
            IncludeTypesNamed(x => x.EndsWith("Endpoint"));
        }

        public void IncludeTypesNamed(Expression<Func<string, bool>> filter)
        {
            var typeParam = Expression.Parameter(typeof(Type), "type"); // type =>
            var nameProp = Expression.Property(typeParam, "Name");  // type.Name
            var invokeFilter = Expression.Invoke(filter, nameProp); // filter(type.Name)
            var lambda = Expression.Lambda<Func<Type, bool>>(invokeFilter, typeParam); // type => filter(type.Name)

            IncludeTypes(lambda);
        }

        /// <summary>
        /// Find Actions on types that match on the provided filter
        /// </summary>
        public void IncludeTypes(Expression<Func<Type, bool>> filter)
        {
            _typeFilters.Includes += filter;
        }

        /// <summary>
        /// Find Actions on concrete types assignable to T
        /// </summary>
        public void IncludeTypesImplementing<T>()
        {
            IncludeTypes(type => !type.IsOpenGeneric() && type.IsConcreteTypeOf<T>());
        }

        /// <summary>
        /// Actions that match on the provided filter will be added to the runtime. 
        /// </summary>
        public void IncludeMethods(Expression<Func<MethodInfo, bool>> filter)
        {
            _methodFilters.Includes += filter;
        }

        /// <summary>
        /// Exclude types that match on the provided filter for finding Actions
        /// </summary>
        public void ExcludeTypes(Expression<Func<Type, bool>> filter)
        {
            _typeFilters.Excludes += filter;
        }

        /// <summary>
        /// Actions that match on the provided filter will NOT be added to the runtime. 
        /// </summary>
        public void ExcludeMethods(Expression<Func<MethodInfo, bool>> filter)
        {
            _methodFilters.Excludes += filter;
        }

        /// <summary>
        /// Ignore any methods that are declared by a super type or interface T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void IgnoreMethodsDeclaredBy<T>()
        {
            _methodFilters.IgnoreMethodsDeclaredBy<T>();
        }

        /// <summary>
        /// Exclude any types that are not concrete
        /// </summary>
        public void ExcludeNonConcreteTypes()
        {
            _typeFilters.Excludes += type => !type.IsConcrete();
        }
    }
}