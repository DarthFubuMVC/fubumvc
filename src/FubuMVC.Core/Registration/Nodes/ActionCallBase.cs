using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Routes;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Registration.Nodes
{
    public abstract class ActionCallBase : BehaviorNode, DescribesItself, IModifiesChain
    {
        public Type HandlerType { get; protected set; }
        public MethodInfo Method { get; protected set; }

        public bool HasInput
        {
            get { return Method.GetParameters().Length > 0; }
        }

        public bool HasOutput
        {
            get { return Method.ReturnType != typeof (void); }
        }

        public ActionCallBase(Type handlerType, MethodInfo method)
        {
            Next = null;


            setHandlerAndMethod(handlerType, method);
        }

        protected ActionCallBase()
        {
        }

        protected void setHandlerAndMethod(Type handlerType, MethodInfo method)
        {
            HandlerType = handlerType;
            Method = method;
        }

        public bool IsAsync => Method.ReturnType.CanBeCastTo<Task>();

        public override string Description => "{0}.{1}({2}) : {3}".ToFormat(HandlerType.Name, Method.Name, getInputParameters(),
            HasOutput ? Method.ReturnType.Name : "void");

        private string getInputParameters()
        {
            if (! HasInput) return "";

            return Method.GetParameters().Select(p => "{0} {1}".ToFormat(p.ParameterType.Name, p.Name)).Join(", ");
        }

        public bool Returns<T>()
        {
            return OutputType().CanBeCastTo<T>();
        }

        protected override IConfiguredInstance buildInstance()
        {
            Validate();

            var invocationType = typeof(ActionInvocation<>).MakeGenericType(HandlerType);
            var invocation = Activator.CreateInstance(invocationType, Method);

            var instance = new ConfiguredInstance(typeof(ActionInvoker<>), HandlerType);
            instance.Dependencies.Add(invocationType, invocation);


            return instance;
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

            if (HasInput && InputType().IsValueType)
            {
                throw new FubuException(1006,
                    "The type of the input parameter of action '{0}' is a value type (struct). An action must either have no input parameters, or it must have one that is a reference type (class).",
                    Description);
            }
        }


        private Type getOutputTypeOrVoidTaskResult()
        {
            var genericArguments = Method.ReturnType.GetGenericArguments();
            if (genericArguments.Length == 0)
                return Method.ReturnType;
            return genericArguments.First();
        }

        public Type OutputType()
        {
            if (Method.ReturnType == typeof (void)) return null;

            return Method.ReturnType.CanBeCastTo<Task>()
                ? getOutputTypeOrVoidTaskResult()
                : Method.ReturnType;
        }

        public Type InputType()
        {
            return HasInput ? Method.GetParameters().First().ParameterType : null;
        }


        public Type ResourceType()
        {
            return OutputType();
        }


        void DescribesItself.Describe(Description description)
        {
            var shortTitle = "{0}.{1}()".ToFormat(HandlerType.Name, Method.Name);

            description.Title = shortTitle;
            description.Properties["Handler Type"] = HandlerType.FullName;
            description.Properties["Assembly"] = HandlerType.Assembly.GetName().Name;
            description.Properties["Method"] = Method.Name;


            if (InputType() != null)
            {
                description.Properties["Input Type"] = InputType().FullName;
            }

            if (ResourceType() != null)
            {
                description.Properties["Resource Type"] = ResourceType().FullName;
            }
        }


        public void Modify(BehaviorChain chain)
        {
            var outputType = OutputType();


            if (outputType == null) return;

            if (outputType.CanBeCastTo<FubuContinuation>() || outputType.CanBeCastTo<IRedirectable>())
            {
                AddAfter(new ContinuationNode());
            }
        }

        public void ForAttributes<T>(Action<T> action) where T : Attribute
        {
            HandlerType.ForAttribute(action);
            Method.ForAttribute(action);
            if (HasInput)
            {
                InputType().ForAttribute(action);
            }
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return HandlerType.HasAttribute<T>() || Method.HasAttribute<T>() || (HasInput && InputType().HasAttribute<T>());
        }
    }
}