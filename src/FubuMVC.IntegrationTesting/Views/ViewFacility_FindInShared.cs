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

            InBottle("BottleA");
            RazorView("Shared/Foo");
            RazorView("Shared/Application");
            RazorView("Bar"); // NOT shared

            InBottle("BottleB");
            RazorView("Shared/Foo");
            RazorView("Shared/Bar");
        }

        [Test]
        public void application_wins_if_the_file_exists_in_multiple_places()
        {
            RazorViews.FindInShared("Application").Origin.ShouldEqual(ContentFolder.Application);
        }

        [Test]
        public void find_the_file_in_the_bottle_order()
        {
            RazorViews.FindInShared("Foo").Origin.ShouldEqual("BottleA");
        }

        [Test]
        public void search_through_all_bottles_to_find_shared()
        {
            RazorViews.FindInShared("Bar").Origin.ShouldEqual("BottleB");
        }
    }
}