using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;

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

            var chain1 = new RoutedChain("something");
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

            var chain1 = new RoutedChain(""){
                UrlCategory ={
                    Category = null
                }
            };

            var chain2 = new RoutedChain("")
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

            var chain1 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new RoutedChain("")
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

            var chain1 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chain3 = new RoutedChain("")
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

            var chain1 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "else"
                }
            };

            var chain3 = new RoutedChain("")
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

            var chain1 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chain3 = new RoutedChain("")
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
        public void find_by_category_strict_with_multiple_chains_1()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Strict,
                CategoryOrHttpMethod = "something"
            };

            var chain1 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            var chain2 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chain3 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = null
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2, chain3 };

            search.FindForCategory(chains).ShouldHaveTheSameElementsAs(chain1);
        }

        [Test]
        public void find_by_category_strict_with_multiple_chains_by_method()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Strict,
                CategoryOrHttpMethod = "POST"
            };

            var chain1 = new RoutedChain("whatever")
            {
                UrlCategory =
                {
                    Category = "something"
                }
            };

            chain1.Route.AllowedHttpMethods.Add("POST");

            var chain2 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chain3 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = null
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2, chain3 };

            search.FindForCategory(chains).ShouldHaveTheSameElementsAs(chain1);
        }

        [Test]
        public void find_by_category_strict_with_multiple_chains_2()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Strict,
                CategoryOrHttpMethod = "something"
            };

            var chain1 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "different"
                }
            };

            var chain2 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = Categories.DEFAULT
                }
            };

            var chain3 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = "else"
                }
            };

            var chains = new BehaviorChain[] { chain1, chain2, chain3 };

            search.FindForCategory(chains).Any().ShouldBeFalse();
        }


        [Test]
        public void find_by_category_strict_with_only_one_chain_that_does_not_match_still_returns_nothing()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Strict,
                CategoryOrHttpMethod = "something"
            };



            var chain3 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = null
                }
            };

            var chains = new BehaviorChain[] { chain3 };

            search.FindForCategory(chains).Any().ShouldBeFalse();
        }

        [Test]
        public void find_by_category_relaxed_with_only_one_chain()
        {
            var search = new ChainSearch
            {
                CategoryMode = CategorySearchMode.Relaxed,
                CategoryOrHttpMethod = "something"
            };


            var chain3 = new RoutedChain("")
            {
                UrlCategory =
                {
                    Category = null
                }
            };

            var chains = new BehaviorChain[] { chain3 };

            search.FindForCategory(chains).ShouldHaveTheSameElementsAs(chain3);
        }



    }

    [TestFixture]
    public class finding_behavior_chains_by_type_only
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = BehaviorGraph.BuildFrom<FakeRegistry>();
        }

        [Test]
        public void find_candidates_by_input_model_only_when_its_strict()
        {
            new ChainSearch{
                Type = typeof (SingleActionController),
                TypeMode = TypeSearchMode.InputModelOnly
            }.FindCandidatesByType(theGraph).Single().Any().ShouldBeFalse();
        }

        [Test]
        public void find_candidates_by_type_fall_back_to_handler_type_if_possible()
        {
            var chains = new ChainSearch{
                Type = typeof (SingleActionController),
                TypeMode = TypeSearchMode.Any
            }.FindCandidatesByType(theGraph).SelectMany(x => x);


            chains.Single()
                .FirstCall().Description.ShouldEqual("SingleActionController.DoSomething(InputModel model) : void");
        }

        [Test]
        public void find_by_handler_type_only()
        {

            new ChainSearch{
                TypeMode = TypeSearchMode.HandlerOnly,
                Type = typeof (SimpleInputModel)
            }.FindCandidatesByType(theGraph).Single().Single().FirstCall().Description.ShouldEqual("SimpleInputModel.DoSomething(InputModel2 model) : void");
        }

        [Test]
        public void find_by_input_model_only()
        {
            var chainSearch = new ChainSearch
                              {
                                  TypeMode = TypeSearchMode.InputModelOnly,
                                  Type = typeof(SimpleInputModel)
                              };

            Debug.WriteLine(chainSearch);


            chainSearch.FindCandidatesByType(theGraph).Single().Select(x => x.FirstCall().Description)
            .ShouldHaveTheSameElementsAs("OneController.Query(SimpleInputModel model) : SimpleOutputModel", "TwoController.NotQuery(SimpleInputModel model) : SimpleOutputModel");
        }

        [Test]
        public void find_by_any_looks_at_input_model_first_then_handler_type_second()
        {
            var candidates = new ChainSearch{
                TypeMode = TypeSearchMode.Any,
                Type = typeof (SimpleInputModel)
            }.FindCandidatesByType(theGraph);

            candidates.Count().ShouldEqual(2);

            candidates.First().Select(x => x.FirstCall().Description)
                .ShouldHaveTheSameElementsAs("OneController.Query(SimpleInputModel model) : SimpleOutputModel", "TwoController.NotQuery(SimpleInputModel model) : SimpleOutputModel");

            candidates.Last().Single().FirstCall().Description.ShouldEqual("SimpleInputModel.DoSomething(InputModel2 model) : void");
        }

        [Test]
        public void find_by_method_if_it_exists()
        {
            var candidates = new ChainSearch
            {
                TypeMode = TypeSearchMode.Any,
                Type = typeof(SimpleInputModel),
                MethodName = "DoSomething"
            }.FindCandidatesByType(theGraph);



            candidates.First().Any().ShouldBeFalse();
            candidates.Last().Single().FirstCall().Description.ShouldEqual("SimpleInputModel.DoSomething(InputModel2 model) : void");
        }

        [Test]
        public void find_by_method_if_it_exists_2()
        {
            var candidates = new ChainSearch
            {
                TypeMode = TypeSearchMode.Any,
                Type = typeof(SimpleInputModel),
                MethodName = "Query"
            }.FindCandidatesByType(theGraph);



            candidates.First().Select(x => x.FirstCall().Description)
                .ShouldHaveTheSameElementsAs("OneController.Query(SimpleInputModel model) : SimpleOutputModel");

            candidates.Last().Any().ShouldBeFalse();
        }


        public class FakeRegistry : FubuRegistry
        {
            public FakeRegistry()
            {
                Actions
                    .IncludeType<OneController>()
                    .IncludeType<SimpleInputModel>()
                    .IncludeType<TwoController>()
                    .IncludeType<SingleActionController>();
            }
        }


        public class InputModel2{}
        public class SimpleInputModel
        {
            public void DoSomething(InputModel2 model){}
        }

        public class SimpleOutputModel
        {
        }

        public class OneController
        {
            public void Go()
            {
            }

            public SimpleOutputModel Report()
            {
                return new SimpleOutputModel();
            }

            public SimpleOutputModel Query(SimpleInputModel model)
            {
                return new SimpleOutputModel();
            }
        }

        public class TwoController
        {
            public void Go()
            {
            }

            public SimpleOutputModel Report()
            {
                return new SimpleOutputModel();
            }

            public SimpleOutputModel NotQuery(SimpleInputModel model)
            {
                return new SimpleOutputModel();
            }
        }

        public class InputModel1 { }

        public class SingleActionController
        {
            public void DoSomething(InputModel model) { }
        }
    }


}