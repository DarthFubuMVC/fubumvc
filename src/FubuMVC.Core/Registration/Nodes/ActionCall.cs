using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Nodes
{
    /// <summary>
    /// A Fubu-specific representation of a method that can be treated as 
    /// Action. Such a method must adhere to a number of rules e.g. the one-model-in one-model-out pattern.
    /// If you are unsure whether you method is eligible, you can instantiate an ActionCall and
    /// call <see cref="ActionCallBase.Validate"/>
    /// </summary>
    public class ActionCall : ActionCallBase, IMayHaveInputType, IMayHaveResourceType, DescribesItself
    {
        
        public ActionCall(Type handlerType, MethodInfo method) : base(handlerType, method)
        {
        }

        public override BehaviorCategory Category { get { return BehaviorCategory.Call; } }


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

                RouteBuilder.PopulateQuerystringParameters(InputType(), input);

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
            return other.HandlerType.FullName.Equals(HandlerType.FullName) && other.Method.Matches(Method);
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
                return ((HandlerType != null ? HandlerType.FullName.GetHashCode() : 0)*397) ^
                       (Method != null ? Method.Name.GetHashCode() : 0);
            }
        }



        public IRouteDefinition BuildRouteForPattern(string pattern)
        {
            return HasInput
               ? RouteBuilder.Build(InputType(), pattern)
               : new RouteDefinition(pattern);
        }

        public BehaviorChain BuildChain(UrlPolicies urlPolicies)
        {
            var chain = buildChain(urlPolicies);

            chain.AddToEnd(this);

            ForAttributes<ModifyChainAttribute>(att => att.Alter(this));

            return chain;
        }

        private BehaviorChain buildChain(UrlPolicies urlPolicies)
        {
            if (HasAttribute<FubuPartialAttribute>() || Method.Name.EndsWith("Partial"))
            {
                return new BehaviorChain
                {
                    IsPartialOnly = true
                };
            }
            else
            {
                var route = urlPolicies.BuildRoute(this);

                return new RoutedChain(route, InputType(), ResourceType());
            }
        }
    }
}