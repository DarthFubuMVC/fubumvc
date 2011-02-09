using FubuMVC.Core;

namespace Spark.Web.FubuMVC
{
	public class SparkPackageExtension : IFubuRegistryExtension
	{
		public void Configure(FubuRegistry registry)
		{
			registry.WithSparkDefaults();
		}
	}
}