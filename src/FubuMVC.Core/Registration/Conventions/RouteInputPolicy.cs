using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class RouteInputPolicy : IRouteInputPolicy
    {
        private readonly Alteration<IRouteDefinition, ActionCall> _inputBuilders;
        private readonly Alteration<IRouteDefinition, PropertyInfo> _propertyAlterations;
        private readonly CompositeFilter<InputPropertyForAction> _propertyFilters = new CompositeFilter<InputPropertyForAction>();

        public RouteInputPolicy()
        {
            _inputBuilders = new Alteration<IRouteDefinition, ActionCall>(addInputs);
            _propertyAlterations = new Alteration<IRouteDefinition, PropertyInfo>(addPropertyInput);
        }

        public bool Matches(Type inputType)
        {
            return true;
        }

        public void AlterRoute(IRouteDefinition route, ActionCall call)
        {
            _inputBuilders.Alter(route, call);
        }

        private void addInputs(IRouteDefinition route, ActionCall call)
        {
            call.InputType().GetProperties()
                .Where(p => _propertyFilters.Matches(new InputPropertyForAction(call, p)))
                .Each(prop => _propertyAlterations.Alter(route, prop));
        }

        private static void addPropertyInput(IRouteDefinition route, PropertyInfo property)
        {
            var input = new RouteParameter(new SingleProperty(property));
            route.Input.AddRouteInput(input, true);
        }

        public CompositeFilter<InputPropertyForAction> PropertyFilters { get { return _propertyFilters; } }
        public Alteration<IRouteDefinition, ActionCall> InputBuilders { get { return _inputBuilders; } }
        public Alteration<IRouteDefinition, PropertyInfo> PropertyAlterations { get { return _propertyAlterations; } }
    }

    public class InputPropertyForAction
    {
        public InputPropertyForAction(ActionCall call, PropertyInfo prop)
        {
            Call = call;
            InputProperty = prop;
        }

        public ActionCall Call { get; set;}
        public PropertyInfo InputProperty { get; set; }
    }
}