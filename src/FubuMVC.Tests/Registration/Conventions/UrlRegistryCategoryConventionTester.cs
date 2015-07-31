using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.Urls;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class UrlRegistryCategoryConventionTester
    {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetUp()
        {

            graph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<UrlCategoryController1>();
                x.Actions.IncludeType<UrlCategoryController2>();
                x.Actions.IncludeType<UrlCategoryController3>();
                    
            });

            var request = OwinHttpRequest.ForTesting().FullUrl("http://server/app");
			var urlResolver = new ChainUrlResolver(request);

			registry = new UrlRegistry(new ChainResolutionCache(graph), urlResolver, request);
        }

        [Test]
        public void action_that_has_no_attribute_on_either_class_or_method_should_have_no_category()
        {
            graph.ChainFor<UrlCategoryController3>(x => x.Go(null)).As<RoutedChain>().UrlCategory.Category.ShouldBeNull();
            graph.ChainFor<UrlCategoryController3>(x => x.Comeback()).As<RoutedChain>().UrlCategory.Category.ShouldBeNull();

        }

        [Test]
        public void class_attribute_sets_the_category_if_there_is_no_method_level_attribute()
        {
            graph.ChainFor<UrlCategoryController1>(x => x.Go(null)).As<RoutedChain>().UrlCategory.Category.ShouldBe("admin");
        }

        [Test]
        public void method_attribute_has_precedence_over_class_attribute()
        {
            graph.ChainFor<UrlCategoryController1>(x => x.Comeback()).As<RoutedChain>().UrlCategory.Category.ShouldBe("public");
        }

        [Test]
        public void method_attribute_without_class_attribute()
        {
            graph.ChainFor<UrlCategoryController2>(x => x.DecoratedMethod()).As<RoutedChain>().UrlCategory.Category.ShouldBe("public");
        }
        
        [Test]
        public void finds_and_registers_url_for_new_methods()
        {
            registry.HasNewUrl(typeof(UrlNewTarget)).ShouldBeTrue();
            registry.UrlForNew(typeof(UrlNewTarget)).ShouldBe("/urlcategory1/createnewtarget");
            
        }

        #endregion

        private BehaviorGraph graph;
        private UrlRegistry registry;
    }

    public class UrlNewTarget
    {
        
    }

    

    [UrlRegistryCategory("admin")]
    public class UrlCategoryController1
    {
        [UrlForNew(typeof(UrlNewTarget))]
        public string CreateNewTarget()
        {
            return "new one!";
        }

        public void Go(InputModel model)
        {
        }

        [UrlRegistryCategory("public")]
        public string Comeback()
        {
            return "over here";
        }
    }

    public class UrlCategoryController2
    {
        public void Go()
        {
        }

        [UrlRegistryCategory("public")]
        public string DecoratedMethod()
        {
            return "decorated";
        }
    }

    public class UrlCategoryController3
    {
        public void Go(InputModel model)
        {
        }

        public string Comeback()
        {
            return "over here";
        }
    }
}