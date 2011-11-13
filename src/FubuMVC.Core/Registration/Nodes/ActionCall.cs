using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    /// A Fubu-specific representation of a method that can be treated as 
    /// Action. Such a method must adhere to a number of rules e.g. the one-model-in one-model-out pattern.
    /// If you are unsure whether you method is eligible, you can instantiate an ActionCall and
    /// call <see cref="ActionCallBase.Validate"/>
    /// </summary>
    public class ActionCall : ActionCallBase, IMayHaveInputType
    {
        
        public ActionCall(Type handlerType, MethodInfo method) : base(handlerType, method)
        {
        }

        public override BehaviorCategory Category { get { return BehaviorCategory.Call; } }

        public void ForAttributes<T>(Action<T> action) where T : Attribute
        {
            HandlerType.ForAttribute(action);
            Method.ForAttribute(action);
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return HandlerType.HasAttribute<T>() || Method.HasAttribute<T>();
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