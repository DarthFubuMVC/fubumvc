using System;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using System.Collections.Generic;
using FubuMVC.Core.View.Activation;
using FubuMVC.Core.View.Attachment;
using FubuMVC.Core.View.New;

namespace FubuMVC.Core.Registration.DSL
{
    public class ViewExpression
    {
        private readonly ConfigurationGraph _configuration;
        private readonly FubuRegistry _registry;

        public ViewExpression(ConfigurationGraph configuration, FubuRegistry registry)
        {
            _configuration = configuration;
            _registry = registry;
        }

        /// <summary>
        /// Register a view facility.
        /// </summary>
        public ViewExpression Facility(IViewFacility facility)
        {
            _configuration.AddFacility(facility);
            return this;
        }

        /// <summary>
        /// Configure actionless views for view tokens matching the specified filter
        /// </summary>
        public ViewExpression RegisterActionLessViews(Func<IViewToken, bool> viewTokenFilter)
        {
            return RegisterActionLessViews(viewTokenFilter, chain => { chain.IsPartialOnly = true; });
        }

        /// <summary>
        /// Specify which views should be treated as actionless views.
        /// </summary>
        /// <param name="viewTokenFilter"></param>
        /// <param name="configureChain">Continuation for configuring each generated <see cref="BehaviorChain"/></param>
        /// <returns></returns>
        public ViewExpression RegisterActionLessViews(Func<IViewToken, bool> viewTokenFilter, Action<BehaviorChain> configureChain)
        {
            _configuration.AddConvention(new ActionLessViewConvention(viewTokenFilter, configureChain));
            return this;          
        }

        /// <summary>
        /// Specify which views should be treated as actionless views.
        /// </summary>
        /// <param name="viewTokenFilter"></param>
        /// <param name="configureChain">Continuation for configuring each generated <see cref="BehaviorChain"/>, depending on the corresponding view token</param>
        /// <returns></returns>
		public ViewExpression RegisterActionLessViews(Func<IViewToken, bool> viewTokenFilter, Action<BehaviorChain, IViewToken> configureChain)
        {
            _configuration.AddConvention(new ActionLessViewConvention(viewTokenFilter, configureChain));
            return this;          
        }

        /// <summary>
        /// Fine-tune the view attachment instead of using <see cref="TryToAttachWithDefaultConventions"/>
        /// </summary>
        public ViewExpression TryToAttach(Action<ViewsForActionFilterExpression> configure)
        {
            var convention = new ViewAttacherConvention();

            var expression = new ViewsForActionFilterExpression(convention);
            configure(expression);

            _configuration.AddPolicy(convention);

            return this;
        }

        /// <summary>
        /// Configures the view attachment mechanism with default conventions:
        /// a) by_ViewModel_and_Namespace_and_MethodName
        /// b) by_ViewModel_and_Namespace
        /// c) by_ViewModel
        /// </summary>
        /// <returns></returns>
        public ViewExpression TryToAttachWithDefaultConventions()
        {
            return TryToAttach(x =>
            {
                x.by_ViewModel_and_Namespace_and_MethodName();
                x.by_ViewModel_and_Namespace();
                x.by_ViewModel();
            });
        }


        /// <summary>
        /// Define a view activation policy for views matching the filter.
        /// <seealso cref="IfTheInputModelOfTheViewMatches"/>
        /// </summary>
        public PageActivationExpression IfTheViewTypeMatches(Func<Type, bool> filter)
        {
            Action<IPageActivationSource> registration = source => _registry.Services(x => x.AddService<IPageActivationSource>(source));
            return new PageActivationExpression(registration, filter);
        }

        /// <summary>
        /// Define a view activation policy by matching on the input type of a view.
        /// A view activation element implements <see cref="IPageActivationAction"/> and takes part in setting up a View instance correctly
        /// at runtime.
        /// </summary>
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

    public class ActionLessViewConvention : IConfigurationAction
    {
        private readonly Func<IViewToken, bool> _viewTokenFilter;
        private readonly Action<BehaviorChain, IViewToken> _configureChain;

        public ActionLessViewConvention(Func<IViewToken, bool> viewTokenFilter, Action<BehaviorChain> configureChain)
        {
            _viewTokenFilter = viewTokenFilter;
            _configureChain = (chain, token) => configureChain(chain);
        }

		public ActionLessViewConvention(Func<IViewToken, bool> viewTokenFilter, Action<BehaviorChain, IViewToken> configureChain)
        {
            _viewTokenFilter = viewTokenFilter;
            _configureChain = configureChain;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Views
                .Views
                .Where(token => _viewTokenFilter(token))
                .Each(token =>
                          {
                              var chain = BehaviorChain.ForWriter(new ViewNode(token));
                              graph.AddChain(chain);

                              _configureChain(chain, token);
                          });
        }
    }
}