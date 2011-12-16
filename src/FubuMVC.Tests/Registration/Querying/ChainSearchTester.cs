using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

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
            new ChainSearch().CategoryOrHttpMethod.ShouldEqual(Categories.DEFAULT);
        }

        [Test]
        public void find_by_category_when_the_category_is_null_and_relaxed_search_and_only_one_chain()
        {
            var search = new ChainSearch{
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = null
            };

            var chains = new BehaviorChain[]{new BehaviorChain(),};

            search.FindForCategory(chains).Single().ShouldBeTheSameAs(chains.Single());
        }


        [Test]
        public void find_by_category_when_the_category_is_null_and_relaxed_search_and_only_one_chain_2()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = null
            };

            var chain1 = new BehaviorChain();
            chain1.UrlCategory.Category = Categories.DEFAULT;

            var chains = new BehaviorChain[] { chain1, };

            search.FindForCategory(chains).Single().ShouldBeTheSameAs(chains.Single());
        }

        [Test]
        public void find_by_null_category_with_multiple_chains_but_only_one_is_default()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = null
            };

            var chain1 = new BehaviorChain{
                UrlCategory ={
                    Category = null
                }
            };

            var chain2 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2};

            search.FindForCategory(chains).Single().ShouldBeTheSameAs(chain2);
        }

        [Test]
        public void find_by_null_category_with_multiple_chains_but_only_one_is_default_2()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = null
            };

            var chain1 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2 };

            search.FindForCategory(chains).Single().ShouldBeTheSameAs(chain2);
        }

        [Test]
        public void find_by_null_category_with_multiple_chains_but_only_one_is_default_3()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = null
            };

            var chain1 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chain3 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2, chain3 };

            search.FindForCategory(chains).ShouldHaveTheSameElementsAs(chain2, chain3);
        }

        [Test]
        public void find_by_null_category_with_multiple_chains_but_none_is_marked_default()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = null
            };

            var chain1 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = "else"
                }
            };

            var chain3 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = null
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2, chain3 };

            search.FindForCategory(chains).ShouldHaveTheSameElementsAs(chain3);
        }

        [Test]
        public void find_by_null_category_with_multiple_chains_one_null_category_one_default_strict_category_search()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Strict,
                CategoryOrHttpMethod = null
            };

            var chain1 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chain3 = new BehaviorChain
            {
                UrlCategory =
                {
                    Category = null
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2, chain3 };

            search.FindForCategory(chains).ShouldHaveTheSameElementsAs(chain3);
        }
    }
}