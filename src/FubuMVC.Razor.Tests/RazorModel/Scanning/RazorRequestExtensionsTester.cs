using FubuCore;
using FubuMVC.Core.View.Model.Scanning;
using FubuMVC.Razor.RazorModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests.RazorModel.Scanning
{
	[TestFixture]	
	public class RazorRequestExtensionsTester : InteractionContext<ScanRequest>
    {
		[Test]
	    public void include_razor_views_adds_correct_filter()
	    {
			ClassUnderTest.IncludeRazorViews();
			ClassUnderTest.Filters.ShouldContain("*{0}".ToFormat(".cshtml"));
	    }
	}
}
