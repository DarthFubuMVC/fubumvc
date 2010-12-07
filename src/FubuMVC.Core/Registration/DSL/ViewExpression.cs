using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewExpression
    {
        private readonly ViewAttacher _viewAttacher;
        private readonly FubuRegistry _registry;

        public ViewExpression(ViewAttacher viewAttacher, FubuRegistry registry)
        {
            _viewAttacher = viewAttacher;
            _registry = registry;
        }

        public ViewExpression Facility(IViewFacility facility)
        {
            _viewAttacher.AddFacility(facility);
            return this;
        }

        public ViewExpression RegisterActionLessViews(Func<Type, bool> viewTypeFilter)
        {
            return RegisterActionLessViews(viewTypeFilter, chain => { });
        }

        public ViewExpression RegisterActionLessViews(Func<Type, bool> viewTypeFilter, Action<BehaviorChain> configureChain)
        {
            _registry.ApplyConvention(new ActionLessViewConvention(_viewAttacher.Facilities, _viewAttacher.Types, viewTypeFilter, configureChain));
            return this;          
        }

        public ViewExpression TryToAttach(Action<ViewsForActionFilterExpression> configure)
        {
            var expression = new ViewsForActionFilterExpression(_viewAttacher);
            configure(expression);

            return this;
        }

        public ViewExpression TryToAttachWithDefaultConventions()
        {
            return TryToAttach(x =>
            {
                x.by_ViewModel_and_Namespace_and_MethodName();
                x.by_ViewModel_and_Namespace();
                x.by_ViewModel();
            });
        }
    }

    public class ActionLessViewConvention : IConfigurationAction
    {
        private readonly IEnumerable<IViewFacility> _facilities;
        private readonly TypePool _types;
        private readonly Func<Type, bool> _viewTypeFilter;
        private readonly Action<BehaviorChain> _configureChain;

        public ActionLessViewConvention(IEnumerable<IViewFacility> facilities, TypePool types, Func<Type, bool> viewTypeFilter, Action<BehaviorChain> configureChain)
        {
            _facilities = facilities;
            _types = types;
            _viewTypeFilter = viewTypeFilter;
            _configureChain = configureChain;
        }

        public void Configure(BehaviorGraph graph)
        {
            // This needs to be forced to true here
            _types.ShouldScanAssemblies = true;
            _types.TypesMatching(_viewTypeFilter).Each(type =>
            {
                var chain = graph.AddChain();
                var node = _facilities.FirstValue(x => x.CreateViewNode(type));
                chain.AddToEnd(node);

                _configureChain(chain);
            });
        }
    }
}