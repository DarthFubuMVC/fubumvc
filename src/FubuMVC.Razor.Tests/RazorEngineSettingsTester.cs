using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Razor.Tests
{
	[TestFixture]
    public class RazorEngineSettingsTester : InteractionContext<RazorEngineSettings>
    {
		[Test]
	    public void includes_razor_views_by_default()
	    {
            ClassUnderTest.Search.Include.Split(';')
                .ShouldHaveTheSameElementsAs("*cshtml", "*vbhtml");
	    }

        [Test]
        public void uses_deep_search_by_default()
        {
            ClassUnderTest.Search.DeepSearch.ShouldBeTrue();
        }
	}
}
