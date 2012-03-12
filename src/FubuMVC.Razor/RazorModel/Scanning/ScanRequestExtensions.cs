using FubuCore;
using FubuMVC.Core.View.Model.Scanning;

namespace FubuMVC.Razor.RazorModel.Scanning
{
	public static class ScanRequestExtensions
	{
        public static void IncludeRazorViews(this ScanRequest request)
		{
			var csPattern = "*{0}".ToFormat(".cshtml");
			var vbPattern = "*{0}".ToFormat(".vbhtml");
			request.Include(csPattern);
			request.Include(vbPattern);
        }
    }
}
