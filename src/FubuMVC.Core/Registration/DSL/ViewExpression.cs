using System;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using System.Collections.Generic;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewExpression
    {
        private readonly ViewBagConventionRunner _viewAttacher;
        private readonly ViewAttacherConvention _viewAttacherConvention;
        private readonly FubuRegistry _registry;

        public ViewExpression(ViewBagConventionRunner viewAttacher, FubuRegistry registry, ViewAttacherConvention viewAttacherConvention)
        {
            _viewAttacher = viewAttacher;
            _viewAttacherConvention = viewAttacherConvention;
            _registry = registry;
        }

        public ViewExpression Facility(IViewFacility facility)
        {
            _viewAttacher.AddFacility(facility);
            return this;
        }

        public ViewExpression RegisterActionLessViews(Func<IViewToken, bool> viewTypeFilter)
        {
            return RegisterActionLessViews(viewTypeFilter, chain => { });
        }

        public ViewExpression RegisterActionLessViews(Func<IViewToken, bool> viewTypeFilter, Action<BehaviorChain> configureChain)
        {
            _viewAttacher.Apply(new ActionLessViewConvention(viewTypeFilter, configureChain));
            return this;          
        }

        public ViewExpression TryToAttach(Action<ViewsForActionFilterExpression> configure)
        {
            var expression = new ViewsForActionFilterExpression(_viewAttacherConvention);
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

        public ViewExpression TryToAttachViewsInPackages()
        {
            _registry.ConfigureImports(i =>
            {
                var importAttacher = i.Views._viewAttacher;
                var importConvention = i.Views._viewAttacherConvention;

                if(!importAttacher.Facilities.Any())
                {
                    _viewAttacherConvention
                        .Filters
                        .Each(importConvention.AddViewsForActionFilter);
                }

                _viewAttacher
                    .Facilities
                    .Each(importAttacher.AddFacility);
            });

            return this;
        }

        /// <summary>
        /// Define a view activation policy
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PageActivationExpression IfTheViewTypeMatches(Func<Type, bool> filter)
        {
            Action<IPageActivationSource> registration = source => _registry.Services(x => x.AddService<IPageActivationSource>(source));
            return new PageActivationExpression(registration, filter);
        }

        /// <summary>
        /// Define a view activation policy by matching on the input type of a view
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PageActivationExpression IfTheInputModelOfTheViewMatches(Func<Type, bool> filter)
        {
            Func<Type, bool> combined = type =>
            {
                var inputType = type.InputModel();
                return inputType == null ? false : filter(inputType);
            };

            return IfTheViewTypeMatches(combined);
        }
    }

    public class ActionLessViewConvention : IViewBagConvention
    {
        private readonly Func<IViewToken, bool> _viewTypeFilter;
        private readonly Action<BehaviorChain> _configureChain;

        public ActionLessViewConvention(Func<IViewToken, bool> viewTypeFilter, Action<BehaviorChain> configureChain)
        {
            _viewTypeFilter = viewTypeFilter;
            _configureChain = configureChain;
        }

        public void Configure(ViewBag bag, BehaviorGraph graph)
        {
            bag
                .Views
                .Where(token => _viewTypeFilter(token))
                .Each(token =>
                          {
                              var chain = graph.AddChain();
                              var output = token.ToBehavioralNode();
                              chain.AddToEnd(output);

                              _configureChain(chain);
                          });
        }
    }
}