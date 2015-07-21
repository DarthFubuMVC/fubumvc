using System;

namespace FubuMVC.Core.ServiceBus.Runtime.Routing
{
    public interface IRoutingRule
    {
        bool Matches(Type type);
        string Describe();
    }
}