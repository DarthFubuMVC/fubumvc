using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    // TODO:  Blow up if there are no filters of any sort
    public class BehaviorMatcher
    {
        private readonly Func<Type, MethodInfo, ActionCall> _actionCallProvider;
        private readonly CompositeFilter<ActionCall> _methodFilters = new CompositeFilter<ActionCall>();
        private readonly CompositeFilter<Type> _typeFilters = new CompositeFilter<Type>();
        private BehaviorGraph _graph;

        public BehaviorMatcher(Func<Type, MethodInfo, ActionCall> actionCallProvider)
        {
            _actionCallProvider = actionCallProvider;
            _methodFilters.Excludes += method => method.Method.DeclaringType == typeof (object);
            _methodFilters.Excludes += method => method.Method.DeclaringType == typeof (MarshalByRefObject);
            _methodFilters.Excludes += method => method.Method.DeclaringType == typeof (IDisposable);
            _methodFilters.Excludes += method => method.Method.ContainsGenericParameters;

            _methodFilters.Excludes += method => method.Method.IsSpecialName;

            _methodFilters.ResetChangeTracking();
            _typeFilters.ResetChangeTracking();
        }

        public CompositeFilter<Type> TypeFilters { get { return _typeFilters; } set { } }
        public CompositeFilter<ActionCall> MethodFilters { get { return _methodFilters; } set { } }

        public bool HasFilters()
        {
            return _methodFilters.HasChanged || _typeFilters.HasChanged;
        }

        public void BuildBehaviors(TypePool pool, BehaviorGraph graph)
        {
            _graph = graph;

            // Do not do any assembly scanning if no type or method filters are set
            pool.ShouldScanAssemblies = HasFilters();

            pool.TypesMatching(TypeFilters.Matches).Each(scanMethods);
            _graph = null;
        }


        private void scanMethods(Type type)
        {
            type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => _actionCallProvider(type, x))
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