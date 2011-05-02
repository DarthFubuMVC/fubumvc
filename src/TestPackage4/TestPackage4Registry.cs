using FubuMVC.Core;

namespace TestPackage4
{
	public class TestPackage4Registry : FubuPackageRegistry, IFubuRegistryExtension
	{
		public void Configure(FubuRegistry registry)
		{
            registry.Import(this, "pak4");
		}
	}
}