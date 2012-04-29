using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
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

        public IOrderPolicyExpression WrapBehaviorChainsWith<T>() where T : IActionBehavior
        {
            var configAction = new VisitBehaviorsAction(v =>
                v.Actions += chain => chain.Prepend(new Wrapper(typeof (T))),
                "wrap with the {0} behavior".ToFormat(typeof(T).Name));

            return applyWrapper<T>(configAction);
        }

        private IOrderPolicyExpression applyWrapper<T>(VisitBehaviorsAction configAction)
        {
            _configuration.AddPolicy(configAction);

            _lastNodeMatch = ReorderBehaviorsPolicy.FuncForWrapper(typeof(T));

            return this;
        }

        public IOrderPolicyExpression ConditionallyWrapBehaviorChainsWith<T>(Expression<Func<ActionCall, bool>> filter) where T : IActionBehavior
        {
            var reason = "wrap with the {0} behavior if [{1}]".ToFormat(typeof(T).Name, filter.Body.ToString());
            var chainFilter = filter.Compile();
            var configAction = new VisitBehaviorsAction(v => 
                {
                    v.Filters += chain => chain.FirstCall() != null && chainFilter(chain.FirstCall());
                    v.Actions += chain => chain.Prepend(new Wrapper(typeof(T)));
                }, reason);

            return applyWrapper<T>(configAction);
        }

        private void addPolicy(Action<BehaviorGraph> action)
        {
            var policy = new LambdaConfigurationAction(action);
            _configuration.AddPolicy(policy);
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
            _configuration.AddPolicy(alteration);
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

            // Not unit tested and therefore, not real code yet.
            //public void MustBeBeforeBehavior<T>() where T : IActionBehavior
            //{
            //    var policy = new ReorderBehaviorsPolicy(){
            //        WhatMustBeBefore = _lastNodeMatch
            //    };

            //    policy.ThisWrapperMustBeAfter<T>();
            //    _systemPolicies.Add(policy);
            //}

            public void MustBeBeforeAuthorization()
            {
                var policy = new ReorderBehaviorsPolicy(){
                    WhatMustBeBefore = _lastNodeMatch
                };

                policy.ThisNodeMustBeAfter<AuthorizationNode>();
                _configuration.AddPolicy(policy);
            }
        }
    }
}