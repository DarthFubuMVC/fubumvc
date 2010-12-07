using System.Linq;
using FubuCore.Binding;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ServiceRegistryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void ClearAll()
        {
            var registry1 = new HtmlConventionRegistry();
            var registry2 = new HtmlConventionRegistry();
            var registry3 = new HtmlConventionRegistry();
            var registry4 = new HtmlConventionRegistry();

            var services = new ServiceRegistry();
            services.AddService(registry1);
            services.AddService(registry2);
            services.AddService(registry3);
            services.AddService(registry4);

            services.ClearAll<HtmlConventionRegistry>();

            services.FindAllValues<HtmlConventionRegistry>().Any().ShouldBeFalse();
        }

        [Test]
        public void fill_service_that_has_something_already_does_not_add_values()
        {
            var registry1 = new HtmlConventionRegistry();
            var registry2 = new HtmlConventionRegistry();

            var services = new ServiceRegistry();
            services.AddService(registry1);

            services.SetServiceIfNone(registry2);

            services.FindAllValues<HtmlConventionRegistry>().ShouldHaveTheSameElementsAs(registry1);
        }

        [Test]
        public void fill_service_with_nothing_should_add_the_service()
        {
            var registry1 = new HtmlConventionRegistry();

            var services = new ServiceRegistry();
            services.SetServiceIfNone(registry1);

            services.FindAllValues<HtmlConventionRegistry>().ShouldHaveTheSameElementsAs(registry1);
        }

        [Test]
        public void GetAllValues()
        {
            var registry1 = new HtmlConventionRegistry();
            var registry2 = new HtmlConventionRegistry();
            var registry3 = new HtmlConventionRegistry();
            var registry4 = new HtmlConventionRegistry();

            var services = new ServiceRegistry();
            services.AddService(registry1);
            services.AddService(registry2);
            services.AddService(registry3);
            services.AddService(registry4);

            services.AddService<HtmlConventionRegistry, HtmlConventionRegistry>();

            services.FindAllValues<HtmlConventionRegistry>()
                .ShouldHaveTheSameElementsAs(registry1, registry2, registry3, registry4);
        }

        [Test]
        public void should_be_singleton_is_true_for_any_type_ending_in_Cache()
        {
            ServiceRegistry.ShouldBeSingleton(typeof (IPropertyBinderCache)).ShouldBeTrue();
            ServiceRegistry.ShouldBeSingleton(typeof (IModelBinderCache)).ShouldBeTrue();
        }
    }
}