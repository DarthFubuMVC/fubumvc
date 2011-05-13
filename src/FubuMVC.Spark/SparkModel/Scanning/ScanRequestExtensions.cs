using System.Collections.Generic;
using FubuCore;
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
		
		public static void AddRoots(this ScanRequest request, IEnumerable<string> roots)
		{
			roots.Each(request.AddRoot);
        }
    }
}
