using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.Media.Formatters;

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
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            var call = new ActionCall(typeof (C), method);

            _outputType = call.OutputType();

            return returnChain(call);
        }

        public ChainedBehaviorExpression OutputToJson()
        {
            var outputNode = new ConnegOutputNode(_outputType);
            outputNode.UseFormatter<JsonFormatter>();

            return returnChain(outputNode);
        }

        public ChainedBehaviorExpression OutputTo(OutputNode node)
        {
            return returnChain(node);
        }

        private ChainedBehaviorExpression returnChain(BehaviorNode node)
        {
            _registration(node);
            return new ChainedBehaviorExpression(node.AddAfter)
            {
                _outputType = _outputType
            };
        }
    }
}