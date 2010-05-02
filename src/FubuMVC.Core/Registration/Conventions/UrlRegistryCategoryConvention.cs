using System;
using System.Collections.Generic;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Conventions
{
    public class UrlRegistryCategoryConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Each(a =>
            {
                a.HandlerType.ForAttribute<UrlRegistryCategoryAttribute>(att => a.ParentChain().Route.Category = att.Category);
                a.Method.ForAttribute<UrlRegistryCategoryAttribute>(att => a.ParentChain().Route.Category = att.Category);
            });
        }
    }
}