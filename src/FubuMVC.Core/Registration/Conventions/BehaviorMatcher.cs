using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    // TODO:  Blow up if there are no filters of any sort
    public class BehaviorMatcher
    {
        private readonly CompositeFilter<ActionCall> _methodFilters = new CompositeFilter<ActionCall>();
        private readonly CompositeFilter<Type> _typeFilters = new CompositeFilter<Type>();
        private BehaviorGraph _graph;

        public BehaviorMatcher()
        {
            _methodFilters.Excludes += method => method.Method.DeclaringType == typeof (object);
            _methodFilters.Excludes += method => method.Method.ContainsGenericParameters;

            _methodFilters.Excludes += method => method.Method.IsSpecialName;
        }

        public CompositeFilter<Type> TypeFilters { get { return _typeFilters; } set { } }
        public CompositeFilter<ActionCall> MethodFilters { get { return _methodFilters; } set { } }

        public void BuildBehaviors(TypePool pool, BehaviorGraph graph)
        {
            _graph = graph;

            pool.TypesMatching(TypeFilters.Matches).Each(scanMethods);
            _graph = null;
        }


        private void scanMethods(Type type)
        {
            type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => new ActionCall(type, x))
                .Where(MethodFilters.Matches)
                .Each(registerBehavior);
        }

        private void registerBehavior(ActionCall call)
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(call);
            _graph.AddChain(chain);
        }
    }
}