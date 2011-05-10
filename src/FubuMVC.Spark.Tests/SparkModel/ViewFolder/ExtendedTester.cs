using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel.ViewFolder
{
    [TestFixture]
    public class ExtendedTester
    {
        private const string Package1 = "Package1";
        private const string Package2 = "Package2";

        private readonly TemplateViewFolder _viewFolder;
        private readonly ISparkViewEngine _engine;

        private readonly IEnumerable<ITemplate> _pak1Templates;
        private readonly IEnumerable<ITemplate> _pak2Templates;
        private readonly IEnumerable<ITemplate> _appTemplates;

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
            
            var allTemplates = new List<Template>();
            allTemplates.AddRange(scanner.FindInPackages());
            allTemplates.AddRange(scanner.FindInHost());

            var viewPathPolicy = new ViewPathPolicy();
            allTemplates.Each(viewPathPolicy.Apply);

            _viewFolder = new TemplateViewFolder(allTemplates);
            _engine = new SparkViewEngine { ViewFolder = _viewFolder };

            _pak1Templates = new List<ITemplate>(allTemplates.ByOrigin(Package1));
            _pak2Templates = new List<ITemplate>(allTemplates.ByOrigin(Package2));
            _appTemplates = new List<ITemplate>(allTemplates.ByOrigin(FubuSparkConstants.HostOrigin));
        }

        [Test]
        public void host_views_are_located_correctly()
        {
            var one = _appTemplates.FirstByName("MacBook");
            var footer = _appTemplates.FirstByName("_footer");

            getViewSource(one).ShouldEqual("MacBook");
            getViewSource(footer).ShouldEqual("This is the footer");
        }


        [Test]
        public void deployed_package_views_are_located_correctly()
        {
            var uno = _pak1Templates.FirstByName("SerieSL");
            var header = _pak1Templates.FirstByName("_header");

            getViewSource(uno).ShouldEqual("<appname/> SerieSL");
            getViewSource(header).ShouldEqual("Lenovo Header");
        }


        [Test]
        public void dev_package_views_are_located_correctly()
        {
            var uno = _pak2Templates.FirstByName("Vostro");
            var header = _pak2Templates.FirstByName("_footer");

            getViewSource(uno).ShouldEqual("<appname/> Vostro");
            getViewSource(header).ShouldEqual("Dell footer");
        }

        [Test]
        public void the_correct_number_of_templates_are_resolved()
        {
            _appTemplates.ShouldHaveCount(9);
            _pak1Templates.ShouldHaveCount(9);
            _pak2Templates.ShouldHaveCount(8);
        }

        [Test]
        public void views_with_same_path_are_resolved_correctly()
        {
            var hostView = _appTemplates.FirstByName("_samePath");
            var pak1View = _pak1Templates.FirstByName("_samePath");
            var pak2View = _pak2Templates.FirstByName("_samePath");

            getViewSource(hostView).ShouldEqual("Host _samePath.spark");
            getViewSource(pak1View).ShouldEqual("Package1 _samePath.spark");
            getViewSource(pak2View).ShouldEqual("Package2 _samePath.spark");
        }

        [Test]
        public void views_from_packages_can_refer_to_other_views_from_the_same_package()
        {
            var tresView = _pak1Templates.FirstByName("SerieW");
            var treView = _pak2Templates.FirstByName("Xps");

            getViewSource(tresView).ShouldEqual("<header/> SerieW");
            renderTemplate(tresView).ShouldEqual("Lenovo Header SerieW");
            
            getViewSource(treView).ShouldEqual("Xps <footer/>");
            renderTemplate(treView).ShouldEqual("Xps Dell footer");
        }


        [Test]
        public void views_from_host_can_refer_to_other_views_from_the_host()
        {
            var threeView = _appTemplates.FirstByName("MacPro");

            getViewSource(threeView).ShouldEqual("<header/> MacPro");
            renderTemplate(threeView).ShouldEqual("This is the header MacPro");
        }

        [Test]
        public void host_views_are_isolated_from_packages()
        {
            var noLuck = _appTemplates.FirstByName("NoLuck");
            getViewSource(noLuck).ShouldEqual("Will <fail/>");
            renderTemplate(noLuck).ShouldEqual("Will <fail/>");
        }

        [Test]
        public void views_from_packages_are_isolated_among_packages()
        {
            var dosView = _pak1Templates.FirstByName("SerieT");
            var dueView = _pak2Templates.FirstByName("Inspiron");

            getViewSource(dosView).ShouldEqual("SerieT <dell/>");
            renderTemplate(dosView).ShouldEqual("SerieT <dell/>");
            
            getViewSource(dueView).ShouldEqual("Inspiron <lenovo/>");
            renderTemplate(dueView).ShouldEqual("Inspiron <lenovo/>");
        }

        [Test]
        public void views_from_packages_can_refer_views_from_top_level_shared_directory_in_host()
        {
            var pak1UnoView = _pak1Templates.FirstByName("SerieSL");
            var pak2UnoView = _pak2Templates.FirstByName("Vostro");

            getViewSource(pak1UnoView).ShouldEqual("<appname/> SerieSL");
            renderTemplate(pak1UnoView).ShouldEqual("Computers Catalog SerieSL");

            getViewSource(pak2UnoView).ShouldEqual("<appname/> Vostro");
            renderTemplate(pak2UnoView).ShouldEqual("Computers Catalog Vostro");
        }

        [Test]
        public void views_from_packages_can_use_masters_from_the_same_package()
        {
            var cuatroView = _pak1Templates.FirstByName("SerieX");
            var master = _pak1Templates.FirstByName("Maker");

            getViewSource(cuatroView).ShouldEqual("<use master=\"Maker\"/> SerieX");           
            renderTemplate(cuatroView, master).ShouldEqual("Lenovo SerieX");
        }

        private string getViewSource(ITemplate template)
        {
            var content = _viewFolder.GetViewSource(template.ViewPath);
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
            templates = templates ?? Enumerable.Empty<Template>().ToArray();
            var descriptor = new SparkViewDescriptor();
            descriptor.AddTemplate(template.ViewPath);
            templates.Each(x => descriptor.AddTemplate(x.ViewPath));

            var instance = _engine.CreateInstance(descriptor);
            var writer = new StringWriter();
            instance.RenderView(writer);
            return writer.ToString();
        }
    }
}