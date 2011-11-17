using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration
{

    public class ActionSource : IActionSource
    {
        private readonly ActionMethodFilter _methodFilters;
        private readonly CompositeFilter<Type> _typeFilters = new CompositeFilter<Type>();
        private readonly CompositeFilter<ActionCall> _callFilters = new CompositeFilter<ActionCall>();

        public ActionSource(ActionMethodFilter methodFilters)
        {
            _methodFilters = methodFilters;
        }

        public ActionMethodFilter MethodFilters
        {
            get { return _methodFilters; }
        }

        public CompositeFilter<Type> TypeFilters
        {
            get { return _typeFilters; }
        }

        public CompositeFilter<ActionCall> CallFilters
        {
            get { return _callFilters; }
        }

        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
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

        public void IncludeTypesNamed(Expression<Func<string, bool>> filter)
        {
            var typeParam = Expression.Parameter(typeof(Type), "type"); // type =>
            var nameProp = Expression.Property(typeParam, "Name");  // type.Name
            var invokeFilter = Expression.Invoke(filter, nameProp); // filter(type.Name)
            var lambda = Expression.Lambda<Func<Type, bool>>(invokeFilter, typeParam); // type => filter(type.Name)

            IncludeTypes(lambda);
        }

        public void IncludeTypes(Expression<Func<Type, bool>> filter)
        {
            TypeFilters.Includes += filter;
        }
    }
}