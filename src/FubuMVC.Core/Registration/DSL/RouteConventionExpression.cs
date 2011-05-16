using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.DSL
{
    public class RouteConventionExpression
    {
        private readonly FubuRegistry _registry;
        private readonly RouteDefinitionResolver _resolver;

        public RouteConventionExpression(RouteDefinitionResolver resolver, FubuRegistry registry)
        {
            _resolver = resolver;
            _registry = registry;
        }

        public RouteConventionExpression IgnoreNamespaceText(string nameSpace)
        {
            _resolver.DefaultUrlPolicy.IgnoreNamespace(nameSpace);
            return this;
        }



        public RouteConventionExpression IgnoreMethodsNamed(string methodName)
        {
            _resolver.DefaultUrlPolicy.IgnoreMethods(methodName);
            return this;
        }

        public RouteConventionExpression IgnoreClassSuffix(string suffix)
        {
            _resolver.DefaultUrlPolicy.IgnoreClassSuffix(suffix);
            return this;
        }

        public RouteConventionExpression IgnoreClassNameForType<T>()
        {
            _resolver.DefaultUrlPolicy.IgnoreClassName(typeof(T));
            return this;
        }

        public RouteConventionExpression IgnoreSpecificInputForInputTypeAndMethod<T>(
            Func<ActionCall, bool> methodFilter, Expression<Func<T, object>> propertyFilter)
        {
            Accessor prop = ReflectionHelper.GetAccessor(propertyFilter);
            _resolver.InputPolicy.PropertyFilters.Excludes.Add(
                input => methodFilter(input.Call) && input.InputProperty.Name == prop.InnerProperty.Name);
            return this;
        }

        public RouteConventionExpression IgnoreInputsForInputTypeAndMethod<T>(Func<ActionCall, bool> filter)
        {
            _resolver.InputPolicy.InputBuilders.Register(filter, (r, t) => { });
            return this;
        }

        public RouteConventionExpression IgnoreControllerFolderName()
        {
            _resolver.DefaultUrlPolicy.IgnoreControllerFolderName = true;
            return this;
        }

        public RouteConventionExpression IgnoreControllerNamesEntirely()
        {
            _resolver.DefaultUrlPolicy.IgnoreControllerNamesEntirely = true;
            return this;
        }

        public RouteConventionExpression IgnoreControllerNamespaceEntirely()
        {
            _resolver.DefaultUrlPolicy.IgnoreControllerNamespaceEntirely = true;
            return this;
        }

        public RouteConventionExpression AppendClassesWith(Func<ActionCall, bool> filter, string pattern)
        {
            _resolver.DefaultUrlPolicy.AppendClassesWith(filter, pattern);
            return this;
        }

        public RouteConventionExpression AppendAllClassesWith(string pattern)
        {
            _resolver.DefaultUrlPolicy.AppendClassesWith(x=>true,pattern);
            return this;
        }

        public RouteConventionExpression ModifyRouteDefinitions(Func<ActionCall, bool> filter, Action<IRouteDefinition> modification)
        {
            _resolver.DefaultUrlPolicy.RegisterRouteModification(filter, modification);
            return this;
        }

        public RouteConventionExpression ConstrainToHttpMethod(Expression<Func<ActionCall, bool>> filter, string method)
        {
            _resolver.ConstraintPolicy.AddHttpMethodFilter(filter, method);
            return this;
        }

        public RouteConventionExpression ForInputTypesOf<T>(Action<IInputTypeRouteInputsModel<T>> configure)
        {
            var inputs = new InputTypeRouteInputsModel<T>();
            configure(inputs);

            _resolver.InputPolicy.InputBuilders.Register(call => call.InputType().CanBeCastTo<T>(),
                                                         (r, t) => inputs.Modify(r));

            return this;
        }

        public RouteMethodAlteration<T> ForInputTypesAndMethods<T>(Func<ActionCall, bool> filter)
        {
            var alteration = new RouteMethodAlteration<T>(filter);
            _registry.Policies.Add(alteration);

            return alteration;
        }


        public RouteConventionExpression IgnoreMethodSuffix(string suffix)
        {
            _resolver.DefaultUrlPolicy.IgnoreMethodSuffix(suffix);
            return this;
        }

        public RouteConventionExpression UrlPolicy<T>() where T : IUrlPolicy, new()
        {
            return UrlPolicy(new T());
        }

        public RouteConventionExpression UrlPolicy(IUrlPolicy policy)
        {
            _resolver.RegisterUrlPolicy(policy);
            return this;
        }

        public RouteConventionExpression HomeIs<TController>(Expression<Action<TController>> controllerAction)
        {
            var method = ReflectionHelper.GetMethod(controllerAction);
            _resolver.RegisterUrlPolicy(new DefaultRouteMethodBasedUrlPolicy(method), true);
            return this;
        }

        public RouteConventionExpression HomeIs<TInputModel>()
        {
            _resolver.RegisterUrlPolicy(new DefaultRouteInputTypeBasedUrlPolicy(typeof (TInputModel)), true);
            return this;
        }

        #region Nested type: RouteMethodAlteration

        public class RouteMethodAlteration<T> : IConfigurationAction
        {
            private readonly Func<ActionCall, bool> _filter;
            private readonly InputTypeRouteInputsModel<T> _modification = new InputTypeRouteInputsModel<T>();

            public RouteMethodAlteration(Func<ActionCall, bool> filter)
            {
                _filter = filter;
            }

            void IConfigurationAction.Configure(BehaviorGraph graph)
            {
                graph.Behaviors
                    .Where(x => x.InputType().CanBeCastTo<T>())
                    .Where(x => _filter(x.FirstCall()))
                    .Where(x => x.Route != null).Each(x => _modification.Modify(x.Route));
            }

            public void ModifyRoute(Action<IInputTypeRouteInputsModel<T>> configure)
            {
                configure(_modification);
            }
        }

        #endregion

        /// <summary>
        /// This directs the routing conventions to ignore the default
        /// assembly namespace when creating a route
        /// </summary>
        /// <returns></returns>
        public RouteConventionExpression RootAtAssemblyNamespace()
        {
            var assembly = FubuRegistry.FindTheCallingAssembly();
            return IgnoreNamespaceText(assembly.GetName().Name);
        }

        public RouteConventionExpression IgnoreNamespaceForUrlFrom<T>()
        {
            return IgnoreNamespaceText(typeof(T).Namespace);
        }
    }

    public interface IInputTypeRouteInputsModel<T>
    {
        RouteParameter RouteInputFor(Expression<Func<T, object>> expression);
    }

    // TODO -- need a specific unit test for this little monster
    public class InputTypeRouteInputsModel<T> : IInputTypeRouteInputsModel<T>
    {
        private readonly List<Action<IRouteDefinition>> _modifiers = new List<Action<IRouteDefinition>>();

        public RouteParameter RouteInputFor(Expression<Func<T, object>> expression)
        {
            Accessor accessor = ReflectionHelper.GetAccessor(expression);
            var input = new RouteParameter(accessor);
            _modifiers.Add(r => r.Input.AddRouteInput(input, true));

            return input;
        }

        public void Modify(IRouteDefinition route)
        {
            _modifiers.Each(x => x(route));
        }
    }


    public class RouteTypePolicyExpression
    {
        private readonly Func<ActionCall, bool> _filter;
        private readonly RouteInputPolicy _inputPolicy;
        private readonly RouteConventionExpression _parent;

        public RouteTypePolicyExpression(RouteConventionExpression parent, RouteInputPolicy inputPolicy,
                                         Func<ActionCall, bool> filter)
        {
            _parent = parent;
            _inputPolicy = inputPolicy;
            _filter = filter;
        }

        public RouteConventionExpression RouteInputsAre(Action<IRouteDefinition, ActionCall> configure)
        {
            _inputPolicy.InputBuilders.Register(_filter, configure);

            return _parent;
        }
    }
}