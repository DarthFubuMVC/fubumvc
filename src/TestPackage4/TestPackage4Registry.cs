using FubuMVC.Core;
using FubuMVC.Spark;
using Spark;

namespace TestPackage4
{
	public class TestPackage4Registry : FubuPackageRegistry, IFubuRegistryExtension
	{
	    public TestPackage4Registry()
	    {
	        Applies.ToThisAssembly();

            Actions.IncludeClassesSuffixedWithController();

	        Views
                .TryToAttachViewsInPackages()
                .TryToAttachWithDefaultConventions();

	    }

        //public void Configure(FubuRegistry registry)
        //{
        //    registry.Import(this, "pak4");
        //}
	}
}