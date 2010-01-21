using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ActionCall : BehaviorNode
    {
        public ActionCall(Type handlerType, MethodInfo method)
        {
            HandlerType = handlerType;
            Method = method;
            Next = null;
        }

        public Type HandlerType { get; private set; }
        public MethodInfo Method { get; private set; }

        public bool HasInput { get { return Method.GetParameters().Length > 0; } }

        private bool hasReturn { get { return Method.ReturnType != typeof (void); } }
        public override BehaviorCategory Category { get { return BehaviorCategory.Call; } }
        public string Description { get { return "{0}.{1}()".ToFormat(HandlerType.Name, Method.Name); } }

        public static ActionCall For<T>(Expression<Action<T>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            return new ActionCall(typeof (T), method);
        }

        public bool Returns<T>()
        {
            return OutputType().CanBeCastTo<T>();
        }

        protected override ObjectDef buildObjectDef()
        {
            return new ObjectDef
            {
                Dependencies = new List<IDependency>
                {
                    createLambda()
                },
                Type = determineHandlerType()
            };
        }

        private Type determineHandlerType()
        {
            if (hasReturn && HasInput)
            {
                return typeof (OneInOneOutActionInvoker<,,>)
                    .MakeGenericType(
                    HandlerType,
                    Method.GetParameters().First().ParameterType,
                    Method.ReturnType);
            }

            if (hasReturn && !HasInput)
            {
                return typeof (ZeroInOneOutActionInvoker<,>)
                    .MakeGenericType(
                    HandlerType,
                    Method.ReturnType);
            }

            if (!hasReturn && HasInput)
            {
                return typeof (OneInZeroOutActionInvoker<,>)
                    .MakeGenericType(
                    HandlerType,
                    Method.GetParameters().First().ParameterType);
            }

            throw new ArgumentOutOfRangeException("method", Method,
                                                  "Only methods that support the '1 in 1 out', '1 in 0 out', and '0 in 1 out' patterns are valid here");
        }

        private ValueDependency createLambda()
        {
            object lambda = hasReturn
                                ? FuncBuilder.ToFunc(HandlerType, Method)
                                : FuncBuilder.ToAction(HandlerType, Method);
            return new ValueDependency
            {
                DependencyType = lambda.GetType(),
                Value = lambda
            };
        }

        public Type OutputType()
        {
            return Method.ReturnType;
        }

        public Type InputType()
        {
            return HasInput ? Method.GetParameters().First().ParameterType : null;
        }

        public IRouteDefinition ToRouteDefinition()
        {
            if (!HasInput) return new RouteDefinition(string.Empty);

            try
            {
                Type defType = typeof (RouteDefinition<>).MakeGenericType(InputType());
                return Activator.CreateInstance(defType, string.Empty) as IRouteDefinition;
            }
            catch (Exception e)
            {
                throw new FubuException(1001, e, "Could not create a RouteDefinition<> for {0}",
                                        InputType().AssemblyQualifiedName);
            }
        }

        public override string ToString()
        {
            return string.Format("Call {0}", Description);
        }

        public bool Equals(ActionCall other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.HandlerType, HandlerType) && Equals(other.Method, Method);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ActionCall)) return false;
            return Equals((ActionCall) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HandlerType != null ? HandlerType.GetHashCode() : 0)*397) ^
                       (Method != null ? Method.GetHashCode() : 0);
            }
        }

        public void OutputToJson()
        {
            // TODO -- throw exception if this is called and there is no output
            var json = new RenderJsonNode(OutputType());
            Append(json);
        }
    }
}