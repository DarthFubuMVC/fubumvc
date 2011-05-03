using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Registration
{
    public class ConnectImplementationsServiceRegistrationConvention : IServiceRegistrationConvention
    {
        private readonly Type _openType;

        public ConnectImplementationsServiceRegistrationConvention(Type openType)
        {
            _openType = openType;
        }

        public void Register(IEnumerable<Type> matchedTypes, IServiceRegistry services)
        {
            matchedTypes
                .Where(t => t.Closes(_openType) && t.IsClass && !t.IsAbstract)
                .Each(t => services.FillType(_openType, t));
        }
    }
}