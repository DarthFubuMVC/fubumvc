using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using Gate;

namespace FubuMVC.OwinHost
{
    public class FubuOwinHost
    {
        private readonly IApplicationSource _source;

        public FubuOwinHost(IApplicationSource source)
        {
            _source = source;
        }

        // Um, what should we do with exceptions?
        public void ExecuteRequest(IDictionary<string, object> env, ResultDelegate result, Action<Exception> fault)
        {
            
        }

        private IBehaviorFactory buildApplication()
        {
            var application = _source.BuildApplication();
            return application.Bootstrap().Factory;
        }

        public void Recycle()
        {
            RouteTable.Routes.Clear();
        }
    }
}