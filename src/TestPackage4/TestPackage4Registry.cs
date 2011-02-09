using FubuMVC.Core;

namespace TestPackage4
{
	public class TestPackage4Registry : IFubuRegistryExtension
	{
		public void Configure(FubuRegistry registry)
		{
			registry
				.Applies
				.ToAssemblyContainingType<HelloSparkController>();

			registry
				.Actions
				.IncludeClassesSuffixedWithController();
		}
	}
}