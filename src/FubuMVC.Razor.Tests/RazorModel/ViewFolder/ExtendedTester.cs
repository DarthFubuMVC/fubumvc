using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using FubuMVC.Razor.FileSystem;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.RazorModel.Scanning;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Rhino.Mocks;
using ITemplate = FubuMVC.Razor.RazorModel.ITemplate;

namespace FubuMVC.Razor.Tests.RazorModel.ViewFolder
{
    [TestFixture]
    public class ExtendedTester
    {
        private const string Package1 = "Package1";
        private const string Package2 = "Package2";

        private readonly TemplateRegistry _pak1TemplateRegistry;
        private readonly TemplateRegistry _pak2TemplateRegistry;
        private readonly TemplateRegistry _appTemplateRegistry;

        private readonly ITemplateServiceWrapper _templateService;

        private IServiceLocator _serviceLocator;

        public ExtendedTester()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var pathApp = Path.Combine(testRoot, "App");
            var pathPackage1 = Path.Combine(pathApp, "fubu-packages", "Package1", "WebContent");
            var pathPackage2 = Path.Combine(testRoot, "Package2");

            var packages = new List<IPackageInfo>();
            var pack1 = new PackageInfo(Package1);
            var pack2 = new PackageInfo(Package2);
            pack1.RegisterFolder(BottleFiles.WebContentFolder, pathPackage1);
            pack2.RegisterFolder(BottleFiles.WebContentFolder, pathPackage2);
            packages.Add(pack1);
            packages.Add(pack2);

            var scanner = new TemplateFinder(new FileScanner(), packages) {HostPath = pathApp};
            new DefaultTemplateFinderConventions().Configure(scanner);
            
            var allTemplates = new TemplateRegistry();
            allTemplates.AddRange(scanner.FindInPackages());
            allTemplates.AddRange(scanner.FindInHost());

            var viewPathPolicy = new ViewPathPolicy();
            allTemplates.Each(viewPathPolicy.Apply);

            var config = new TemplateServiceConfiguration {BaseTemplateType = typeof (FubuRazorView)};
            _templateService = new TemplateServiceWrapper(new FubuTemplateService(allTemplates, new TemplateService(config)));

            _pak1TemplateRegistry = new TemplateRegistry(allTemplates.ByOrigin(Package1));
            _pak2TemplateRegistry = new TemplateRegistry(allTemplates.ByOrigin(Package2));
            _appTemplateRegistry = new TemplateRegistry(allTemplates.FromHost());

            _serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
        }

        [Test]
        public void host_views_are_located_correctly()
        {
            var one = _appTemplateRegistry.FirstByName("MacBook");
            var footer = _appTemplateRegistry.FirstByName("_footer");

            getViewSource(one).ShouldEqual("MacBook");
            getViewSource(footer).ShouldEqual("This is the footer");
        }

        [Test]
        public void deployed_package_views_are_located_correctly()
        {
            var uno = _pak1TemplateRegistry.FirstByName("SerieSL");
            var header = _pak1TemplateRegistry.FirstByName("_header");

            getViewSource(uno).ShouldEqual("<appname/> SerieSL");
            getViewSource(header).ShouldEqual("Lenovo Header");
        }

        [Test]
        public void dev_package_views_are_located_correctly()
        {
            var uno = _pak2TemplateRegistry.FirstByName("Vostro");
            var header = _pak2TemplateRegistry.FirstByName("_footer");

            getViewSource(uno).ShouldEqual("<appname/> Vostro");
            getViewSource(header).ShouldEqual("Dell footer");
        }

        [Test]
        public void the_correct_number_of_templates_are_resolved()
        {
            _appTemplateRegistry.ShouldHaveCount(10);
            _pak1TemplateRegistry.ShouldHaveCount(10);
            _pak2TemplateRegistry.ShouldHaveCount(8);
        }

        [Test]
        public void views_with_same_path_are_resolved_correctly()
        {
            var hostView = _appTemplateRegistry.FirstByName("_samePath");
            var pak1View = _pak1TemplateRegistry.FirstByName("_samePath");
            var pak2View = _pak2TemplateRegistry.FirstByName("_samePath");

            getViewSource(hostView).ShouldEqual("Host _samePath.cshtml");
            getViewSource(pak1View).ShouldEqual("Package1 _samePath.cshtml");
            getViewSource(pak2View).ShouldEqual("Package2 _samePath.cshtml");
        }

        [Test]
        public void views_from_packages_can_refer_to_other_views_from_the_same_package()
        {
            var tresView = _pak1TemplateRegistry.FirstByName("SerieW");
            var treView = _pak2TemplateRegistry.FirstByName("Xps");

            getViewSource(tresView).ShouldEqual("<header/> SerieW");
            renderTemplate(tresView).ShouldEqual("Lenovo Header SerieW");
            
            getViewSource(treView).ShouldEqual("Xps <footer/>");
            renderTemplate(treView).ShouldEqual("Xps Dell footer");
        }

        [Test]
        public void views_from_host_can_refer_to_other_views_from_the_host()
        {
            var threeView = _appTemplateRegistry.FirstByName("MacPro");

            getViewSource(threeView).ShouldEqual("@Include(\"header\") <p>MacPro</p>");
            renderTemplate(threeView).ShouldEqual("This is the header MacPro");
        }

        [Test]
        public void host_views_are_isolated_from_packages()
        {
            var noLuck = _appTemplateRegistry.FirstByName("NoLuck");
            getViewSource(noLuck).ShouldEqual("Will <fail/>");
            renderTemplate(noLuck).ShouldEqual("Will <fail/>");
        }

        [Test]
        public void views_from_packages_are_isolated_among_packages()
        {
            var dosView = _pak1TemplateRegistry.FirstByName("SerieT");
            var dueView = _pak2TemplateRegistry.FirstByName("Inspiron");

            getViewSource(dosView).ShouldEqual("SerieT <dell/>");
            renderTemplate(dosView).ShouldEqual("SerieT <dell/>");
            
            getViewSource(dueView).ShouldEqual("Inspiron <lenovo/>");
            renderTemplate(dueView).ShouldEqual("Inspiron <lenovo/>");
        }

        [Test]
        public void views_from_packages_can_refer_views_from_top_level_shared_directory_in_host()
        {
            var pak1UnoView = _pak1TemplateRegistry.FirstByName("SerieSL");
            var pak2UnoView = _pak2TemplateRegistry.FirstByName("Vostro");

            getViewSource(pak1UnoView).ShouldEqual("<appname/> SerieSL");
            renderTemplate(pak1UnoView).ShouldEqual("Computers Catalog SerieSL");

            getViewSource(pak2UnoView).ShouldEqual("<appname/> Vostro");
            renderTemplate(pak2UnoView).ShouldEqual("Computers Catalog Vostro");
        }

        [Test]
        public void views_from_packages_can_use_masters_from_the_same_package()
        {
            var cuatroView = _pak1TemplateRegistry.FirstByName("SerieX");
            var master = _pak1TemplateRegistry.FirstByName("Maker");

            getViewSource(cuatroView).ShouldEqual("@{ _Layout = \"Maker\"; } SerieX");           
            renderTemplate(cuatroView, master).ShouldEqual("Lenovo\r\n SerieX");
        }

        private string getViewSource(ITemplate template)
        {
            var content = new FileSystemViewFile(template.FilePath);
            using (var stream = content.OpenViewStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string renderTemplate(ITemplate template, params ITemplate[] templates)
        {
            var descriptor = new ViewDescriptor(template);
            descriptor.ViewFile = new FileSystemViewFile(template.FilePath);
            var current = descriptor;
            for(int i = 0; i < templates.Length; ++i)
            {
                var layoutTemplate = templates[i];
                var layout = new ViewDescriptor(layoutTemplate);
                layout.ViewFile = new FileSystemViewFile(layoutTemplate.FilePath);
                layoutTemplate.Descriptor = layout;
                current.Master = templates[i];
                current = layout;
            }

            var modifier = new ViewModifierService(Enumerable.Empty<IViewModifier>());
            var viewFactory = new ViewFactory(descriptor, _templateService, modifier);
            var view = ((RazorEngine.Templating.ITemplate) viewFactory.GetView());
            return view.Run(new ExecuteContext());
        }
    }
}