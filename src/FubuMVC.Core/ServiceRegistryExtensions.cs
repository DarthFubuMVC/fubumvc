using System;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core
{
	public static class ServiceRegistryExtensions
	{
		public static void Scan(this ServiceRegistry services, Action<IAssemblyScanner> scan)
		{
			var scanner = new AssemblyScanner();
			scan(scanner);
			scanner.Configure(services);
		}

	    public static void FillType<TInterface, TConcrete>(this ServiceRegistry registry) where TConcrete : TInterface
	    {
	        registry.FillType(typeof(TInterface), typeof(TConcrete));
	    }
	}
}