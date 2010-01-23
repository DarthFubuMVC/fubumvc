using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DefaultRouteInputTypeBasedUrlPolicy : IUrlPolicy
    {
        private bool _foundCallAlready;
        private readonly Type _inputType;

        public DefaultRouteInputTypeBasedUrlPolicy(Type inputType)
        {
            _inputType = inputType;
        }

        public bool Matches(ActionCall call)
        {
            var result = call.InputType() == _inputType;

            if( result && _foundCallAlready ) 
            {
                throw new FubuException(1003, 
                "Cannot make input type '{0}' the default route as there is more than one action that uses that input type. Either choose a input type that is used by only one action, or use the other overload of '{1}' to specify the actual action method that will be called by the default route.",
                _inputType.Name,
                ReflectionHelper.GetMethod<FubuRegistry>(r=>r.HomeIs<object>()).Name
                );
            }

            if( result ) _foundCallAlready = true;

            return result;
        }

        public IRouteDefinition Build(ActionCall call)
        {
            return call.ToRouteDefinition();
        }
    }
}