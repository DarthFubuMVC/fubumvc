using FubuMVC.Core;
using FubuMVC.Spark;

namespace TestPackage4
{
	public class TestPackage4Registry : FubuPackageRegistry//, IFubuRegistryExtension
	{
	    public TestPackage4Registry()
	    {
	        Applies.ToThisAssembly();

            Actions.IncludeClassesSuffixedWithController();

	        this.UseSpark();

	        Views.TryToAttachWithDefaultConventions();

	    }

        //public void Configure(FubuRegistry registry)
        //{
        //    registry.Import(this, "pak4");
        //}
	}
}