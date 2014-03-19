using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Registration;

namespace FubuMVC.Core.View.Model
{
    public class ViewTypePool
    {
        private readonly Assembly _applicationAssembly;
        private readonly Lazy<TypePool> _types;

        public ViewTypePool(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
            _types = new Lazy<TypePool>(getTypes);
        }

        public Assembly ApplicationAssembly
        {
            get { return _applicationAssembly; }
        }

        private TypePool getTypes()
        {
            var types = new TypePool();
            types.AddAssembly(_applicationAssembly);

            var filter = new CompositeFilter<Assembly>();
            filter.Excludes.Add(a => a.IsDynamic);
            filter.Excludes.Add(a => types.HasAssembly(a));
            filter.Includes += (t => true);

            types.AddSource(() => AppDomain.CurrentDomain.GetAssemblies().Where(filter.MatchesAll));

            return types;
        }



        public Type FindTypeByName(string typeFullName, Assembly defaultAssembly, Action<string> log)
        {
            if (GenericParser.IsGeneric(typeFullName))
            {
                var genericParser = new GenericParser(_types.Value.Assemblies);
                return genericParser.Parse(typeFullName);
            }

            return findClosedTypeByFullName(typeFullName, defaultAssembly, log);
        }

        private Type findClosedTypeByFullName(string typeFullName, Assembly defaultAssembly, Action<string> log)
        {
            var type = defaultAssembly.GetExportedTypes().FirstOrDefault(x => x.FullName == typeFullName);
            if (type != null)
            {
                return type;
            }

            var types = _types.Value.TypesWithFullName(typeFullName);
            var typeCount = types.Count();

            if (typeCount == 1)
            {
                return types.First();
            }

            log("Unable to set view model type : {0}".ToFormat(typeFullName));

            if (typeCount > 1)
            {
                var candidates = types.Select(x => x.AssemblyQualifiedName).Join(", ");
                log("Type ambiguity on: {0}".ToFormat(candidates));
            }

            return null;
        }

        public Assembly FindAssemblyByProvenance(string provenance)
        {
            if (provenance == ContentFolder.Application) return _applicationAssembly;

            return _types.Value.Assemblies.FirstOrDefault(x => x.GetName().Name == provenance) ?? _applicationAssembly;
        }
    }
}