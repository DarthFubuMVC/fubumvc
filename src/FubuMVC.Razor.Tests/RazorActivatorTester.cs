using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Extensibility;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace FubuMVC.Razor.Tests
{
    [TestFixture]
    public class RazorActivatorTester : InteractionContext<RazorActivator>
    {
        private TemplateServiceWrapper _templateService;
        private ITemplateServiceConfiguration _config;

        protected override void beforeEach()
        {
            _config = new TemplateServiceConfiguration();
            _config.Namespaces.Clear();
            var templateService = new FubuTemplateService(new TemplateRegistry<IRazorTemplate>(),  new TemplateService(_config));
            _templateService = new TemplateServiceWrapper(templateService);

            Services.Inject(_config);
            Services.Inject<ITemplateServiceWrapper>(_templateService);
            ClassUnderTest.Activate(Enumerable.Empty<IPackageInfo>(), MockFor<IPackageLog>());
        }

        [Test]
        public void default_namespaces_are_set()
        {
            var useNamespaces = _config.Namespaces;
            useNamespaces.Each(x => Debug.WriteLine(x));

            useNamespaces.ShouldHaveTheSameElementsAs(new[]
            { 
                typeof(VirtualPathUtility).Namespace,
                typeof(FubuRegistryExtensions).Namespace,
                typeof(FubuPageExtensions).Namespace,
                typeof(ContentExtensionGraph).Namespace,
                typeof(HtmlTag).Namespace
            });
        }

        [Test]
        public void default_page_base_type_is_fuburazorview()
        {
            _config.BaseTemplateType.ShouldEqual(typeof(FubuRazorView));
        }
    }
}
