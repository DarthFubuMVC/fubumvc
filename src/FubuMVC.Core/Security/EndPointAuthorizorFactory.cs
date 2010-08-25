using System;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.Security
{
    public class EndPointAuthorizorFactory : IEndPointAuthorizorFactory
    {
        private readonly IServiceLocator _services;

        public EndPointAuthorizorFactory(IServiceLocator services)
        {
            _services = services;
        }

        public IEndPointAuthorizor AuthorizorFor(Guid behaviorId)
        {
            return _services.GetInstance<IEndPointAuthorizor>(behaviorId.ToString());
        }
    }
}