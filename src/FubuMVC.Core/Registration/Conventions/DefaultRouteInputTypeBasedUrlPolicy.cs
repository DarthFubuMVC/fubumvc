using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DefaultRouteInputTypeBasedUrlPolicy : IUrlPolicy
    {
        private readonly Type _inputType;
        private bool _foundCallAlready;

        public DefaultRouteInputTypeBasedUrlPolicy(Type inputType)
        {
            _inputType = inputType;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            return call.ToRouteDefinition();
        }

        public bool Matches(ActionCall call)
        {
            var result = call.InputType() == _inputType;

            if (result && _foundCallAlready)
            {
                throw new FubuException(1003,
                                        "Cannot make input type '{0}' the default route as there is more than one action that uses that input type. Either choose a input type that is used by only one action, or use the other overload of '{1}' to specify the actual action method that will be called by the default route.",
                                        _inputType.Name,
                                        ReflectionHelper.GetMethod<RouteConventionExpression>(r => r.HomeIs<object>()).
                                            Name
                    );
            }

            if (result) _foundCallAlready = true;

            if (result)
            {
                call.Trace("Action '{0}' is the default route since its input type is {1} which was specified in the configuration as the input model for the default route", call.Method.Name, _inputType.Name);
            }

            return result;
        }
    }
}