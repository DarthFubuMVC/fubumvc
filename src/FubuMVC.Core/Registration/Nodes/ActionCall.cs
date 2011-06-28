using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Nodes
{
    public class ActionCall : BehaviorNode, IMayHaveInputType
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

        public bool HasOutput { get { return Method.ReturnType != typeof (void); } }
        public override BehaviorCategory Category { get { return BehaviorCategory.Call; } }
        public string Description { get { return "{0}.{1}({2}) : {3}".ToFormat(HandlerType.Name, Method.Name, getInputParameters(), HasOutput ? Method.ReturnType.Name : "void"); } }

        public void ForAttributes<T>(Action<T> action) where T : Attribute
        {
            HandlerType.ForAttribute(action);
            Method.ForAttribute(action);
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return HandlerType.HasAttribute<T>() || Method.HasAttribute<T>();
        }

        private string getInputParameters()
        {
            if( ! HasInput ) return "";

            return Method.GetParameters().Select(p => "{0} {1}".ToFormat(p.ParameterType.Name, p.Name)).Join(", ");
        }

        public static ActionCall For<T>(Expression<Action<T>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            return new ActionCall(typeof (T), method);
        }

        public static ActionCall For<T>(Expression<Func<T, object>> expression)
        {
            MethodInfo method = ReflectionHelper.GetMethod(expression);
            return new ActionCall(typeof(T), method);
        }

        /// <summary>
        /// This method creates an ActionCall for an action type with only
        /// one public method.  This method will throw an exception for
        /// any actionType with more than one public method
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public static ActionCall For(Type actionType)
        {
            try
            {
                var method = actionType.GetMethods().Single(x => x.DeclaringType != typeof(object));
                return new ActionCall(actionType, method);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentOutOfRangeException("Only actions with one and only one public method can be used in this method");
            }
        }

        public static ActionCall ForOpenType(Type openType, params Type[] parameterTypes)
        {
            var closedType = openType.MakeGenericType(parameterTypes);
            return For(closedType);
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

        public IRouteDefinition ToRouteDefinition()
        {
            if (!HasInput) return RouteDefinition.Empty();

            try
            {
                Type defType = typeof (RouteInput<>).MakeGenericType(InputType());
                var input = (IRouteInput)Activator.CreateInstance(defType, string.Empty);
                return input.Parent;
            }
            catch (Exception e)
            {
                throw new FubuException(1001, e, "Could not create a RouteInput<> for {0}",
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
            return Equals(other.HandlerType, HandlerType) && methodsEqual(other.Method, Method);
        }

        private bool methodsEqual(MethodInfo method1, MethodInfo method2)
        {
            if (method1.Name != method2.Name) return false;
            var parameters = method1.GetParameters();
            if (parameters.Count() != method2.GetParameters().Count()) return false;
            for (int i = 0; i < parameters.Count(); i++)
            {
                if (!parametersEqual(parameters[i], method2.GetParameters()[i])) return false;
            }

            return true;
        }

        private bool parametersEqual(ParameterInfo parameter1, ParameterInfo parameter2)
        {
            return parameter1.Name == parameter2.Name && parameter1.ParameterType == parameter2.ParameterType;
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

        public IRouteDefinition BuildRouteForPattern(string pattern)
        {
            return HasInput
               ? RouteBuilder.Build(InputType(), pattern)
               : new RouteDefinition(pattern);
        }

    }
}