using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration
{
    public class EndpointActionSource : ActionSource
    {
        public EndpointActionSource()
        {
            IncludeClassesSuffixedWithEndpoint();
        }
    }
}