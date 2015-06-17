using System;
using System.IO;

namespace FubuTransportation.Runtime.Routing
{
    public interface IRoutingRule
    {
        bool Matches(Type type);
        string Describe();
    }
}