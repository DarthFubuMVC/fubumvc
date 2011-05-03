using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Registration
{
    public class AddImplementationsServiceRegistrationConvention : IServiceRegistrationConvention
    {
        private readonly Type _pluginType;

        public AddImplementationsServiceRegistrationConvention(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public void Register(IEnumerable<Type> matchedTypes, IServiceRegistry services)
        {
            matchedTypes
                .Where(t => _pluginType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .Each(t => services.FillType(_pluginType, t));
        }
    }
}