using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Formatting;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Registration.DSL
{
    public interface IPoliciesExpression
    {
        IOrderPolicyExpression WrapBehaviorChainsWith<T>() where T : IActionBehavior;
        IOrderPolicyExpression ConditionallyWrapBehaviorChainsWith<T>(Expression<Func<ActionCall, bool>> filter) where T : IActionBehavior;
        IPoliciesExpression EnrichCallsWith<T>(Func<ActionCall, bool> filter) where T : IActionBehavior;
        IPoliciesExpression AlterActions(Action<ActionCall> configure);
        IPoliciesExpression Add(IConfigurationAction alteration);
        IPoliciesExpression Add<T>() where T : IConfigurationAction, new();
    }

    public interface IOrderPolicyExpression : IPoliciesExpression
    {
        IPoliciesExpression Ordering(Action<PoliciesExpression.BehaviorOrderPolicyExpression> ordering);
    }

    


    public class PoliciesExpression : IOrderPolicyExpression
    {
        private readonly ConfigurationGraph _configuration;
        private Func<BehaviorNode, bool> _lastNodeMatch;

        public PoliciesExpression(ConfigurationGraph configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Apply a new behavior re-ordering rule in order to force behaviors to a certain order.
        /// For example, force Authentication behaviors to always be before Authorization behaviors
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public PoliciesExpression Reorder(Action<ReorderBehaviorsPolicy> configure)
        {
            var policy = new ReorderBehaviorsPolicy();
            configure(policy);

            _configuration.AddConfiguration(policy, ConfigurationType.Reordering);

            return this;
        }

        public IOrderPolicyExpression WrapBehaviorChainsWith<T>() where T : IActionBehavior
        {
            return applyWrapper<T>(chain => true);
        }

        private IOrderPolicyExpression applyWrapper<T>(Func<BehaviorChain, bool> filter) where T : IActionBehavior
        {
            _configuration.AddConfiguration(new WrapAction<T>(filter));

            _lastNodeMatch = ReorderBehaviorsPolicy.FuncForWrapper(typeof(T));

            return this;
        }

        private IOrderPolicyExpression applyWrapper<T>(VisitBehaviorsAction configAction)
        {
            _configuration.AddConfiguration(configAction, ConfigurationType.Policy);

            _lastNodeMatch = ReorderBehaviorsPolicy.FuncForWrapper(typeof(T));

            return this;
        }

        public IOrderPolicyExpression ConditionallyWrapBehaviorChainsWith<T>(Expression<Func<ActionCall, bool>> filter) where T : IActionBehavior
        {
            //var reason = "wrap with the {0} behavior if [{1}]".ToFormat(typeof(T).Name, filter.Body.ToString());
            var actionFilter = filter.Compile();
            return applyWrapper<T>(chain => chain.Calls.Any(actionFilter));
        }

        private void addPolicy(Action<BehaviorGraph> action)
        {
            var policy = new LambdaConfigurationAction(action);
            _configuration.AddConfiguration(policy, ConfigurationType.Policy);
        }

        public IPoliciesExpression EnrichCallsWith<T>(Func<ActionCall, bool> filter) where T : IActionBehavior
        {
            addPolicy(graph =>
            {
                graph.Actions().Where(filter).Each(call =>
                {
                    call.AddAfter(Wrapper.For<T>());
                });
            });

            return this;
        }

        public IPoliciesExpression AlterActions(Action<ActionCall> configure)
        {
            addPolicy(graph => graph.Actions().Each(configure));

            return this;
        }

        public IPoliciesExpression Add(IConfigurationAction alteration)
        {
            _configuration.AddConfiguration(alteration, ConfigurationType.Policy);
            return this;
        }

        public IPoliciesExpression Add<T>() where T : IConfigurationAction, new()
        {
            return Add(new T());
        }

        IPoliciesExpression IOrderPolicyExpression.Ordering(Action<BehaviorOrderPolicyExpression> ordering)
        {
            ordering(new BehaviorOrderPolicyExpression(_lastNodeMatch, _configuration));
            return this;
        }

        public class BehaviorOrderPolicyExpression
        {
            private readonly Func<BehaviorNode, bool> _lastNodeMatch;
            private readonly ConfigurationGraph _configuration;

            public BehaviorOrderPolicyExpression(Func<BehaviorNode, bool> lastNodeMatch, ConfigurationGraph configuration)
            {
                _lastNodeMatch = lastNodeMatch;
                _configuration = configuration;
            }

            public void MustBeBeforeAuthorization()
            {
                var policy = new ReorderBehaviorsPolicy(){
                    WhatMustBeBefore = _lastNodeMatch
                };

                policy.ThisNodeMustBeAfter<AuthorizationNode>();
                _configuration.AddConfiguration(policy);
            }
        }

        public void StringConversions<T>() where T : DisplayConversionRegistry, new()
        {
            var conversions = new T();

            addStringConversions(conversions);
        }


        private void addStringConversions(DisplayConversionRegistry conversions)
        {
            var registry = new ServiceRegistry();
            registry.AddService(typeof (DisplayConversionRegistry), ObjectDef.ForValue(conversions));
            _configuration.AddConfiguration(registry, ConfigurationType.Services);
        }

        public void StringConversions(Action<DisplayConversionRegistry> configure)
        {
            var conversions = new DisplayConversionRegistry();
            configure(conversions);

            addStringConversions(conversions);
        }
    }
}