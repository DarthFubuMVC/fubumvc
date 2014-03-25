using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bottles;

namespace FubuMVC.Core.Registration.DSL
{
    public class AppliesToExpression
    {
        private readonly IList<Action<TypePool>> _typePoolConfigurations = new List<Action<TypePool>>(); 

        private Action<TypePool> configure
        {
            set
            {
                _typePoolConfigurations.Add(value);
            }
        }

        internal TypePool BuildPool(Assembly applicationAssembly)
        {
            var types = new TypePool();

            if (_typePoolConfigurations.Any())
            {
                _typePoolConfigurations.Each(x => x(types));
            }
            else
            {
                types.AddAssembly(applicationAssembly);
            }

            return types;
        }

        /// <summary>
        /// Include the calling assembly (i.e., the assembly of your code calling this method)
        /// </summary>
        public void ToThisAssembly()
        {
            ToAssembly(TypePool.FindTheCallingAssembly());
        }

        /// <summary>
        /// Include the given assembly
        /// </summary>
        public void ToAssembly(Assembly assembly)
        {
            configure = pool => pool.AddAssembly(assembly);
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