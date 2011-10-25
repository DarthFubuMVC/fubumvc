using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using GenericEnumerableExtensions = System.Collections.Generic.GenericEnumerableExtensions;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract class ActionCallBase : BehaviorNode
    {
        public Type HandlerType { get; private set; }
        public MethodInfo Method { get; private set; }
        public bool HasInput { get { return Method.GetParameters().Length > 0; } }
        public bool HasOutput { get { return Method.ReturnType != typeof (void); } }
        
        public ActionCallBase(Type handlerType, MethodInfo method)
        {
            HandlerType = handlerType;
            Method = method;
            Next = null;
        }

        public string Description { get { return "{0}.{1}({2}) : {3}".ToFormat(HandlerType.Name, Method.Name, getInputParameters(), HasOutput ? Method.ReturnType.Name : "void"); } }

        private string getInputParameters()
        {
            if( ! HasInput ) return "";

            return GenericEnumerableExtensions.Join((IEnumerable<string>) Method.GetParameters().Select(p => "{0} {1}".ToFormat(p.ParameterType.Name, p.Name)), ", ");
        }

        public bool Returns<T>()
        {
            return OutputType().CanBeCastTo<T>();
        }

        protected override ObjectDef buildObjectDef()
        {
            Validate();

            var objectDef = new ObjectDef(determineHandlerType());
            objectDef.Dependency(createLambda());

            return objectDef;
        }

        public void Validate()
        {
            if (HasOutput && Method.ReturnType.IsValueType)
            {
                throw new FubuException(1004,
                                        "The return type of action '{0}' is a value type (struct). It must be void (no return type) or a reference type (class).",
                                        Description);
            }

            var parameters = Method.GetParameters();
            if (parameters != null && parameters.Length > 1)
            {
                throw new FubuException(1005,
                                        "Action '{0}' has more than one input parameter. An action must either have no input parameters, or it must have one that is a reference type (class).",
                                        Description);
            }

            if( HasInput && InputType().IsValueType )
            {
                throw new FubuException(1006,
                                        "The type of the input parameter of action '{0}' is a value type (struct). An action must either have no input parameters, or it must have one that is a reference type (class).",
                                        Description);
            }
        }

        private Type determineHandlerType()
        {
            if (HasOutput && HasInput)
            {
                return typeof(OneInOneOutActionInvoker<,,>)
                    .MakeGenericType(
                        HandlerType,
                        Method.GetParameters().First().ParameterType,
                        Method.ReturnType);
            }

            if (HasOutput && !HasInput)
            {
                return typeof(ZeroInOneOutActionInvoker<,>)
                    .MakeGenericType(
                        HandlerType,
                        Method.ReturnType);
            }

            if (!HasOutput && HasInput)
            {
                return typeof(OneInZeroOutActionInvoker<,>)
                    .MakeGenericType(
                        HandlerType,
                        Method.GetParameters().First().ParameterType);
            }

            throw new FubuException(1005,
                                    "The action '{0}' is invalid. Only methods that support the '1 in 1 out', '1 in 0 out', and '0 in 1 out' patterns are valid here", Description);
        }

        private ValueDependency createLambda()
        {
            object lambda = HasOutput
                                ? FuncBuilder.ToFunc(HandlerType, Method)
                                : FuncBuilder.ToAction(HandlerType, Method);
            return new ValueDependency(lambda.GetType(), lambda);
        }

        public Type OutputType()
        {
            return Method.ReturnType;
        }

        public Type InputType()
        {
            return HasInput ? Method.GetParameters().First().ParameterType : null;
        }
    }
}