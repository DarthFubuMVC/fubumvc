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
                a.ForAttributes<UrlRegistryCategoryAttribute>(att => a.ParentChain().UrlCategory.Category = att.Category);
            });
        }
    }
}