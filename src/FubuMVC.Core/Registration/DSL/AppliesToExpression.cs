using System;
using System.Reflection;
using Bottles;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Registration.DSL
{
    public class AppliesToExpression
    {
        private readonly TypePool _pool;

        public AppliesToExpression(TypePool pool)
        {
            _pool = pool;
        }

        public AppliesToExpression ToAllPackageAssemblies()
        {
            _pool.AddSource(() => PackageRegistry.PackageAssemblies);
            return this;
        }

        public AppliesToExpression ToThisAssembly()
        {
            return ToAssembly(FubuRegistry.FindTheCallingAssembly());
        }

        public AppliesToExpression ToAssembly(Assembly assembly)
        {
            _pool.AddAssembly(assembly);
            return this;
        }

        public AppliesToExpression ToAssemblyContainingType<T>()
        {
            return ToAssemblyContainingType(typeof (T));
        }

        public AppliesToExpression ToAssemblyContainingType(Type type)
        {
            return ToAssembly(type.Assembly);
        }

        public AppliesToExpression ToAssembly(string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            return ToAssembly(assembly);
        }
    }
}