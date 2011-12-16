using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Querying
{
    [TestFixture]
    public class ChainSearchTester
    {
        [Test]
        public void default_category_search_mode_is_relaxed()
        {
            new ChainSearch().CategoryMode.ShouldEqual(CategorySearchMode.Relaxed);
        }

        [Test]
        public void default_type_search_mode_should_be_any()
        {
            new ChainSearch().TypeMode.ShouldEqual(TypeSearchMode.Any);
        }

        [Test]
        public void the_category_is_default()
        {
            new ChainSearch().Category.ShouldEqual(Categories.DEFAULT);
        }
    }
}