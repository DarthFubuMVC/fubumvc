using FubuMVC.Core;

namespace TestPackage4
{
	public class TestPackage4Registry : FubuRegistry, IFubuRegistryExtension
	{
	    public TestPackage4Registry()
	    {
            Applies
                .ToAssemblyContainingType<HelloSparkController>();

            Actions
                .IncludeClassesSuffixedWithController();
	        
	    }

		public void Configure(FubuRegistry registry)
		{
            registry.Import(this, "testpak4");
		}
	}
}