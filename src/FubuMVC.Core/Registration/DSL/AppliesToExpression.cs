using System;
using System.Reflection;
using Bottles;

namespace FubuMVC.Core.Registration.DSL
{
    public class AppliesToExpression
    {
        private readonly TypePool _pool;

        public AppliesToExpression(TypePool pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Include all assemblies known through packages
        /// that come in as bottles
        /// </summary>
        public AppliesToExpression ToAllPackageAssemblies()
        {
            _pool.AddSource(() => PackageRegistry.PackageAssemblies);
            return this;
        }

        /// <summary>
        /// Include the calling assembly (i.e., the assembly of your code calling this method)
        /// </summary>
        public AppliesToExpression ToThisAssembly()
        {
            return ToAssembly(ConfigurationGraph.FindTheCallingAssembly());
        }

        /// <summary>
        /// Include the given assembly
        /// </summary>
        public AppliesToExpression ToAssembly(Assembly assembly)
        {
            _pool.AddAssembly(assembly);
            return this;
        }

        /// <summary>
        /// Include the assembly containing the provided type
        /// </summary>
        public AppliesToExpression ToAssemblyContainingType<T>()
        {
            return ToAssemblyContainingType(typeof (T));
        }

        /// <summary>
        /// Include the assembly containing the provided type
        /// </summary>
        public AppliesToExpression ToAssemblyContainingType(Type type)
        {
            return ToAssembly(type.Assembly);
        }

        /// <summary>
        /// Include the assembly identified by the provided name.
        /// All restrictions known from <see cref="Assembly.Load(string)"/> apply.
        /// </summary>
        public AppliesToExpression ToAssembly(string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            return ToAssembly(assembly);
        }
    }
}