using System;
using System.Linq;
using System.Reflection;
using FubuCore.Util;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View.Model
{
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
    }
}