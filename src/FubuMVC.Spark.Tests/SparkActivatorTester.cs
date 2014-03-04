using System.Diagnostics;
using System.Linq;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using Spark;
using System.Collections.Generic;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkActivatorTester : InteractionContext<SparkActivator>
    {
        private SparkViewEngine _viewEngine;
        private ISparkSettings _settings;

        protected override void beforeEach()
        {
            _viewEngine = new SparkViewEngine();
            _settings = _viewEngine.Settings;

            var commonViewNamespaces = new CommonViewNamespaces();
            commonViewNamespaces.Add("Foo");
            commonViewNamespaces.Add("Bar");

            Services.Inject(commonViewNamespaces);

            Services.Inject<ISparkViewEngine>(_viewEngine);
            ClassUnderTest.Activate(Enumerable.Empty<IPackageInfo>(), MockFor<IPackageLog>());
        }

        [Test]
        public void default_automatic_encoding_is_set_to_true()
        {
            _settings.AutomaticEncoding.ShouldBeTrue();
        }

        [Test]
        public void default_assemblies_are_set()
        {
            _settings.UseAssemblies.ShouldHaveTheSameElementsAs(new[]
            { 
                typeof(HtmlTag).Assembly.FullName,
                typeof(IPartialInvoker).Assembly.FullName 
            });
        }

        [Test]
        public void default_namespaces_are_set_with_the_namespaces_from_common_view_namespaces()
        {
            var useNamespaces = _settings.UseNamespaces;
            useNamespaces.Each(x => Debug.WriteLine(x));

            useNamespaces.ShouldHaveTheSameElementsAs(new[]
            { 
                typeof(IPartialInvoker).Namespace,
                typeof(VirtualPathUtility).Namespace,
                typeof(SparkViewFacility).Namespace,
                
                typeof(HtmlTag).Namespace,
                "Foo", 
                "Bar"
            });
        }

        [Test]
        public void default_viewfolder_is_templateviewfolder()
        {
            _viewEngine.ViewFolder.ShouldBeOfType<TemplateViewFolder>();
        }

        [Test]
        public void default_page_base_type_is_fubusparkview()
        {
            _viewEngine.DefaultPageBaseType.ShouldEqual(typeof(FubuSparkView).FullName);
        }

        [Test]
        public void default_bindingprovider_is_fububindingsprovider()
        {
            _viewEngine.BindingProvider.ShouldBeOfType<FubuBindingProvider>();
        }
    }
}
