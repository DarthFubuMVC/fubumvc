using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.DSL
{
    public class ChainedBehaviorExpression
    {
        private readonly Action<BehaviorNode> _registration;
        private Type _outputType;

        public ChainedBehaviorExpression(Action<BehaviorNode> registration)
        {
            _registration = registration;
        }

        public ChainedBehaviorExpression Calls<C>(Expression<Action<C>> expression)
        {
            var method = ReflectionHelper.GetMethod(expression);
            var call = new ActionCall(typeof (C), method);

            _outputType = call.OutputType();

            return returnChain(call);
        }

        public ChainedBehaviorExpression OutputTo(OutputNode node)
        {
            return returnChain(node);
        }

        private ChainedBehaviorExpression returnChain(BehaviorNode node)
        {
            _registration(node);
            return new ChainedBehaviorExpression(node.AddAfter){
                _outputType = _outputType
            };
        }
    }
}