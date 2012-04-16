using System;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    public class OutputDeterminationExpression
    {
        private readonly FubuRegistry _registry;

        public OutputDeterminationExpression(FubuRegistry registry)
        {
            _registry = registry;
        }

        /// <summary>
        /// Start setting up Json output for behavior chains
        /// </summary>
        public ActionCallFilterExpression ToJson
        {
            get
            {
                return output(call => call.ParentChain().OutputJson(), "Adding json output node to render json");
            }
        }


        /// <summary>
        /// Provide any instance deriving from <see cref="OutputNode"/> for some <see cref="ActionCall"/>
        /// </summary>
        public ActionCallFilterExpression To(Func<ActionCall, OutputNode> func)
        {
            return output(action =>
            {
                OutputNode node = func(action);
                action.AddToEnd(node);
            }, "Adding output nodes from per-call function");
        }

        public ActionCallFilterExpression ToBehavior<T>() where T : IActionBehavior
        {
            return output(action =>
            {
                var node = OutputNode.For<T>();
                action.AddToEnd(node);
            }, "Adding output nodes from per-call function");
        }

        /// <summary>
        /// Specify a type deriving from <see cref="OutputNode"/> to handle output
        /// </summary>
        public ActionCallFilterExpression To<T>() where T : OutputNode, new()
        {
            return output(action => action.AddToEnd(new T()), "Adding output node '{0}'".ToFormat(typeof (T).Name));
        }

        private ActionCallFilterExpression output(Action<ActionCall> configure, string reason)
        {
            var modification = new ActionCallModification(configure, reason);
            _registry.Policies.Add(modification);
//            _registry.ApplyConvention(modification);

            modification.Filters.Excludes += call => call.HasAnyOutputBehavior();

            return new ActionCallFilterExpression(modification.Filters);
        }
    }
}