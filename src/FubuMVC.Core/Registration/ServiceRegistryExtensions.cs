using System;

namespace FubuMVC.Core.Registration
{
	public static class ServiceRegistryExtensions
	{
		public static void Scan(this IServiceRegistry services, Action<IAssemblyScanner> scan)
		{
			var scanner = new AssemblyScanner();
			scan(scanner);
			scanner.Configure(services);
		}
	}
}