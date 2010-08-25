using System;

namespace FubuMVC.Core.Security
{
    public interface IEndPointAuthorizorFactory
    {
        IEndPointAuthorizor AuthorizorFor(Guid behaviorId);
    }
}