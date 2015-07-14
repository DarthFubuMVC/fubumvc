using FubuMVC.Core.Runtime.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views
{
    [TestFixture]
    public class ViewFacility_FindInShared : ViewIntegrationContext
    {
        public ViewFacility_FindInShared()
        {
            RazorView("Shared/Application");
            RazorView("Shared/Theme");

            RazorView("Shared/Foo");
            RazorView("Shared/Application");
            RazorView("Bar"); // NOT shared

            RazorView("Shared/Foo");
            RazorView("Shared/Bar");
        }

        [Test]
        public void application_wins_if_the_file_exists_in_multiple_places()
        {
            RazorViews.FindInShared("Application").Origin.ShouldEqual(ContentFolder.Application);
        }

    }
}