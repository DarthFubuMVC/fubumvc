using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Services;

namespace FubuMVC.Core.Registration.DSL
{
    public class AppliesToExpression
    {
        private readonly IList<Assembly> _assemblies = new List<Assembly>();

        internal IEnumerable<Assembly> Assemblies
        {
            get { return _assemblies; }
        }

        /// <summary>
        /// Include the calling assembly (i.e., the assembly of your code calling this method)
        /// </summary>
        public void ToThisAssembly()
        {
            ToAssembly(AssemblyFinder.FindTheCallingAssembly());
        }

        /// <summary>
        /// Include the given assembly
        /// </summary>
        public void ToAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }

        /// <summary>
        /// Include the assembly containing the provided type
        /// </summary>
        public void ToAssemblyContainingType<T>()
        {
            ToAssemblyContainingType(typeof (T));
        }

        /// <summary>
        /// Include the assembly containing the provided type
        /// </summary>
        public void ToAssemblyContainingType(Type type)
        {
            ToAssembly(type.Assembly);
        }

        /// <summary>
        /// Include the assembly identified by the provided name.
        /// All restrictions known from <see cref="Assembly.Load(string)"/> apply.
        /// </summary>
        public void ToAssembly(string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            ToAssembly(assembly);
        }
    }
}