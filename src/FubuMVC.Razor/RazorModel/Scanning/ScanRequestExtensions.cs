using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.View.Model.Scanning;

namespace FubuMVC.Razor.RazorModel.Scanning
{
	public static class ScanRequestExtensions
	{
        public static void IncludeRazorViews(this ScanRequest request)
		{
			var pattern = "*{0}".ToFormat(".cshtml");
			request.Include(pattern);
        }
		
		public static void AddRoots(this ScanRequest request, IEnumerable<string> roots)
		{
			roots.Each(request.AddRoot);
        }
    }
}
