using FubuMVC.Core;
using FubuMVC.Spark;
using Spark;

namespace TestPackage4
{
	public class TestPackage4Registry : FubuPackageRegistry
	{
	    public TestPackage4Registry()
	    {
	        Applies.ToThisAssembly();

            Actions.IncludeClassesSuffixedWithController();
	    }
	}
}