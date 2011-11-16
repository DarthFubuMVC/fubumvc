using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    public partial class FubuRegistry
    {
        public void ApplyHandlerConventions()
        {
            ApplyHandlerConventions(GetType());
        }

        public void ApplyHandlerConventions<T>()
            where T : class
        {
            ApplyHandlerConventions(typeof(T));
        }

        public void ApplyHandlerConventions(params Type[] markerTypes)
        {
            ApplyHandlerConventions(markers => new HandlersUrlPolicy(markers), markerTypes);
        }

        public void ApplyHandlerConventions(Func<Type[], HandlersUrlPolicy> policyBuilder, params Type[] markerTypes)
        {
            markerTypes
                .Each(t => Applies
                               .ToAssembly(t.Assembly));

            includeHandlers(markerTypes);

            Routes
                .UrlPolicy(policyBuilder(markerTypes));

            Actions.FindWith(_handlerMatcher);
        }

        private void includeHandlers(params Type[] markerTypes)
        {
            markerTypes.Each(markerType => includeTypes(_handlerMatcher, t => t.Namespace.IsNotEmpty() && t.Namespace.StartsWith(markerType.Namespace)));
            includeMethods(_handlerMatcher, action => action.Method.Name == HandlersUrlPolicy.METHOD);
        }

        private void includeTypes(BehaviorMatcher matcher, Expression<Func<Type, bool>> filter)
        {
            matcher.TypeFilters.Includes += filter;
        }

        private void includeMethods(BehaviorMatcher matcher, Expression<Func<ActionCall, bool>> filter)
        {
            matcher.MethodFilters.Includes += filter;
        }
    }
}