using FubuCore;
using FubuMVC.Core.View.Model.Scanning;
using Spark;

namespace FubuMVC.Spark.SparkModel.Scanning
{
	public static class ScanRequestExtensions
	{
        public static void IncludeSparkViews(this ScanRequest request)
		{
			var pattern = "*{0}".ToFormat(Constants.DotSpark);
			request.Include(pattern);
        }
    }
}
