using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using ReflectionExtensions = FubuCore.Reflection.ReflectionExtensions;

namespace FubuMVC.Core.Registration.Conventions
{
    public class UrlForNewConvention : IUrlRegistrationConvention
    {
        public void Configure(BehaviorGraph graph, IUrlRegistration registration)
        {
            graph.Actions().Each(action =>
            {
                ReflectionExtensions.ForAttribute<UrlForNewAttribute>((ICustomAttributeProvider) action.Method, att =>
                {
                    registration.RegisterNew((ActionCall) action, att.Type);
                });
            });
        }
    }
}