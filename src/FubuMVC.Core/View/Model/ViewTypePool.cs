using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Registration;

namespace FubuMVC.Core.View.Model
{
    // TODO -- optimize this!!!!!!
    public class ViewTypePool
    {
        private static readonly Lazy<TypePool> _types = new Lazy<TypePool>(getTypes);

        public static TypePool Default()
        {
            return _types.Value;
        }

        private static TypePool getTypes()
        {
            var types = new TypePool();
            types.AddAssembly(TypePool.FindTheCallingAssembly());

            var filter = new CompositeFilter<Assembly>();
            filter.Excludes.Add(a => a.IsDynamic);
            filter.Excludes.Add(a => types.HasAssembly(a));
            filter.Includes += (t => true);

            types.AddSource(() => AppDomain.CurrentDomain.GetAssemblies().Where(filter.MatchesAll));

            return types;
        }



        public Type FindTypeByName(string typeFullName, Action<string> log)
        {
            if (GenericParser.IsGeneric(typeFullName))
            {
                var genericParser = new GenericParser(_types.Value.Assemblies);
                return genericParser.Parse(typeFullName);
            }

            return findClosedTypeByFullName(typeFullName, log);
        }

        private Type findClosedTypeByFullName(string typeFullName, Action<string> log)
        {
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
    }
}