using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;

namespace Fubu.Applications
{
    public class ApplicationSourceFinder : IApplicationSourceFinder
    {
        private readonly IApplicationSourceTypeFinder _typeFinder;

        public ApplicationSourceFinder(IApplicationSourceTypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public IApplicationSource FindSource(ApplicationSettings settings, ApplicationStartResponse theResponse)
        {
            Type type = findType(settings, theResponse);
            return type == null ? null : (IApplicationSource)Activator.CreateInstance(type);
        }

        private Type findType(ApplicationSettings settings, ApplicationStartResponse theResponse)
        {
            if (settings.ApplicationSourceName.IsNotEmpty())
            {
                return Type.GetType(settings.ApplicationSourceName);
            }

            var types = _typeFinder.FindApplicationSourceTypes();
            theResponse.ApplicationSourceTypes = types.Select(x => x.AssemblyQualifiedName).ToArray();

            if (!types.Any()) return null;

            return only(types) ?? matchingTypeName(settings, types);
        }

        private static Type only(IEnumerable<Type> types)
        {
            return types.Count() == 1 ? types.Single() : null;
        }

        private static Type matchingTypeName(ApplicationSettings settings, IEnumerable<Type> types)
        {
            return types.FirstOrDefault(x => x.Name == settings.Name);
        }
    }
}