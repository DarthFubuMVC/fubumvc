using FubuCore;
using FubuMVC.Core.View.Model.Scanning;
using Spark;

namespace FubuMVC.Spark.SparkModel.Scanning
{
	public static class ScanRequestExtensions
	{
		public static void IncludeSparkViews(this ScanRequest request)
		{
			var dotSparkPattern = "*{0}".ToFormat(Constants.DotSpark);
			request.Include(dotSparkPattern);
			var dotShadePattern = "*{0}".ToFormat(Constants.DotShade);
			request.Include(dotShadePattern);
		}
	}
}
