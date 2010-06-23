using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{
    public class UrlRegistryCategoryConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Each(a =>
            {
                // Sukant, it's just little bitty stuff like this.  If the Type in HandlerType is
                // decorated with the UrlRegistryCategoryAttribute, call through to the continuation
                // passed into the function
                a.HandlerType.ForAttribute<UrlRegistryCategoryAttribute>(att => a.ParentChain().Route.Category = att.Category);
                a.Method.ForAttribute<UrlRegistryCategoryAttribute>(att => a.ParentChain().Route.Category = att.Category);
            });
        }
    }
}