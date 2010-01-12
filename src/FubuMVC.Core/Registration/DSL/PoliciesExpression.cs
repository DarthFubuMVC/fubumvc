using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.DSL
{
    public class PoliciesExpression
    {
        private readonly IList<IConfigurationAction> _actions;

        public PoliciesExpression(IList<IConfigurationAction> actions)
        {
            _actions = actions;
        }

        public PoliciesExpression WrapBehaviorChainsWith<T>() where T : IActionBehavior
        {
            var visitor = new BehaviorVisitor();
            visitor.Actions += chain => chain.Prepend(new Wrapper(typeof (T)));

            _actions.Fill(visitor);

            return this;
        }

        // TODO -- need a test for this turkey
        public PoliciesExpression EnrichCallsWith<T>(Func<ActionCall, bool> filter) where T : IActionBehavior
        {
            var policy =
                new LambdaConfigurationAction(
                    graph => { graph.Actions().Where(filter).Each(call => { call.InsertDirectlyAfter(Wrapper.For<T>()); }); });


            _actions.Fill(policy);

            return this;
        }

        public PoliciesExpression Add(IConfigurationAction alteration)
        {
            _actions.Fill(alteration);
            return this;
        }

        public PoliciesExpression Add<T>() where T : IConfigurationAction, new()
        {
            if (_actions.Any(x => x is T)) return this;

            return Add(new T());
        }
    }
}