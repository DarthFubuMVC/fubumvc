using System;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core
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