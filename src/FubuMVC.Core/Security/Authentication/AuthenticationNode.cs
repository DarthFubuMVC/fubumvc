using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Security.Authentication
{
    public class AuthenticationNode : Node<AuthenticationNode, AuthenticationChain>, IContainerModel
    {
        private readonly Type _authType;

        public AuthenticationNode(Type authType)
        {
            if (!authType.CanBeCastTo<IAuthenticationStrategy>())
            {
                throw new ArgumentOutOfRangeException("authType", "authType must be assignable to IAuthenticationStrategy");
            }

            _authType = authType;
        }

        public Type AuthType
        {
            get { return _authType; }
        }

        Instance IContainerModel.ToInstance()
        {
            var instance = new ConfiguredInstance(_authType);

            configure(instance);

            return instance;
        }

        protected virtual void configure(IConfiguredInstance instance)
        {
            // Nothing
        }

    }
}