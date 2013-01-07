using System;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core
{
	public static class ServiceRegistryExtensions
	{
        /// <summary>
        /// Use an assembly scanning operation to create service registrations
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scan"></param>
		public static void Scan(this ServiceRegistry services, Action<IAssemblyScanner> scan)
		{
			var scanner = new AssemblyScanner();
			scan(scanner);
			scanner.Configure(services);
		}

        /// <summary>
        /// Adds the concrete type TConcrete as a registered implementation of TInterface if it is not already
        /// registered
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TConcrete"></typeparam>
        /// <param name="registry"></param>
	    public static void FillType<TInterface, TConcrete>(this ServiceRegistry registry) where TConcrete : TInterface
	    {
	        registry.FillType(typeof(TInterface), typeof(TConcrete));
	    }
	}
}