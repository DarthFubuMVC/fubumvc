using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Async;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.Registration.Nodes
{
    public class HandlerCall : ActionCallBase, IMayHaveInputType
    {
        public static bool IsCandidate(MethodInfo method)
        {
            if (method.DeclaringType.Equals(typeof(object))) return false;

            var parameterCount = method.GetParameters() == null ? 0 : method.GetParameters().Length;
            if (parameterCount != 1) return false;

            if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
                return false;

            bool hasOutput = method.ReturnType != typeof(void);
            return !hasOutput || !method.ReturnType.IsValueType;
        }

        public static HandlerCall For(Type openType, Type closedType, string methodName)
        {
            var fullType = openType.MakeGenericType(closedType);
            var method = fullType.GetMethod(methodName);

            if (method == null) throw new ArgumentException("Could not find method named '{0}' in type {1}".ToFormat(methodName, fullType.GetFullName()));

            return new HandlerCall(fullType, method);
        }

        public static HandlerCall For<T>(Expression<Action<T>> method)
        {
            return new HandlerCall(typeof(T), ReflectionHelper.GetMethod(method));
        }

        public HandlerCall(Type handlerType, MethodInfo method)
            : base(handlerType, method)
        {
            if (!IsCandidate(method))
            {
                throw new ArgumentOutOfRangeException("method", method, "The method has to have exactly one input");
            }
        }

        public HandlerCall Clone()
        {
            return new HandlerCall(HandlerType, Method);
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Call; }
        }

        protected override Type determineHandlerType()
        {
            Type messageType = Method.GetParameters().First().ParameterType;

            if (HasOutput && Method.ReturnType == typeof (Task))
            {
                return typeof (AsyncHandlerInvoker<,>)
                    .MakeGenericType(HandlerType, messageType);
            }

            if (HasOutput && Method.ReturnType.Closes(typeof(Task<>)))
            {
                return typeof (CascadingAsyncHandlerInvoker<,,>)
                    .MakeGenericType(HandlerType, messageType, Method.ReturnType.GetGenericArguments().First());
            }

            if (HasOutput && HasInput)
            {
                return typeof (CascadingHandlerInvoker<,,>)
                    .MakeGenericType(
                        HandlerType,
                        messageType,
                        Method.ReturnType);
            }

            if (!HasOutput && HasInput)
            {
                return typeof (SimpleHandlerInvoker<,>)
                    .MakeGenericType(
                        HandlerType,
                        messageType);
            }

            throw new FubuException(1005,
                                    "The action '{0}' is invalid. Only methods that support the '1 in 1 out' or '1 in 0 out' patterns are valid as FubuMVC message handlers",
                                    Description);

        }

        public bool CouldHandleOtherMessageType(Type inputType)
        {
            if (inputType == InputType()) return false;

            return inputType.CanBeCastTo(InputType());
        }

        public void AddClone(HandlerChain chain)
        {
            chain.AddToEnd(Clone());
        }

        public override string ToString()
        {
            return "Handler: " + Description;
        }

        public bool Equals(HandlerCall other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.HandlerType == HandlerType && other.Method.Matches(Method);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(HandlerCall)) return false;
            return Equals((HandlerCall)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HandlerType != null ? HandlerType.GetHashCode() : 0) * 397) ^
                       (Method != null ? Method.GetHashCode() : 0);
            }
        }


    }
}