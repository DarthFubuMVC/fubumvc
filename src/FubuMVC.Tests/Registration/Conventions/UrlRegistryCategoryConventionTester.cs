using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Urls;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class UrlRegistryCategoryConventionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.Applies.ToThisAssembly();
                x.Actions.IncludeTypesNamed(n => n.StartsWith("UrlCategory"));
            }).BuildGraph();

          
        }

        [Test]
        public void action_that_has_no_attribute_on_either_class_or_method_should_have_no_category()
        {
            graph.BehaviorFor<UrlCategoryController3>(x => x.Go()).Route.Category.ShouldBeNull();
            graph.BehaviorFor<UrlCategoryController3>(x => x.Comeback()).Route.Category.ShouldBeNull();

        }

        [Test]
        public void class_attribute_sets_the_category_if_there_is_no_method_level_attribute()
        {
            graph.BehaviorFor<UrlCategoryController1>(x => x.Go()).Route.Category.ShouldEqual("admin");
        }

        [Test]
        public void method_attribute_has_precedence_over_class_attribute()
        {
            graph.BehaviorFor<UrlCategoryController1>(x => x.Comeback()).Route.Category.ShouldEqual("public");
        }

        [Test]
        public void method_attribute_without_class_attribute()
        {
            graph.BehaviorFor<UrlCategoryController2>(x => x.DecoratedMethod()).Route.Category.ShouldEqual("public");
        }

        #endregion

        private BehaviorGraph graph;
    }



    [UrlRegistryCategory("admin")]
    public class UrlCategoryController1
    {
        public void Go()
        {
        }

        [UrlRegistryCategory("public")]
        public void Comeback()
        {
        }
    }

    public class UrlCategoryController2
    {
        public void Go()
        {
        }

        [UrlRegistryCategory("public")]
        public void DecoratedMethod()
        {
        }
    }

    public class UrlCategoryController3
    {
        public void Go()
        {
        }

        public void Comeback()
        {
        }
    }
}