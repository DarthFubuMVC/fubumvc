using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View.WebForms;

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
            return returnChain(new RenderJsonNode(_outputType));
        }

        public ChainedBehaviorExpression RenderWebForm<TWebform>()
        {
            // TODO -- blow up if the web form T is strongly typed to something that 
            // does not match the output type of the output type
            // Do it in validation rules
            

            var node = new WebFormView(typeof(TWebform));

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