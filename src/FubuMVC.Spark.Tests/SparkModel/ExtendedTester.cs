using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

// TODO: Convert this to integration tests

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class ExtendedTester
    {
        private const string Package1 = "Package1";
        private const string Package2 = "Package2";
        private const string Global = "Global";

        private readonly TemplateViewFolder _viewFolder;
        private readonly ISparkViewEngine _engine;

        private readonly TemplateRegistry<ITemplate> _pak1TemplateRegistry;
        private readonly TemplateRegistry<ITemplate> _pak2TemplateRegistry;
        private readonly TemplateRegistry<ITemplate> _appTemplateRegistry;
        private readonly TemplateRegistry<ITemplate> _globalTemplateRegistry;
        private readonly TemplateDirectoryProvider<ITemplate> _templateDirectoryProvider;
        private readonly SharingGraph _sharingGraph;

        public ExtendedTester()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var pathApp = Path.Combine(testRoot, "App");
            var pathPackage1 = Path.Combine(pathApp, "fubu-packages", "Package1", "WebContent");
            var pathPackage2 = Path.Combine(testRoot, "Package2");
            var globalPackage = Path.Combine(testRoot, "Global");

            var templateRegistry = new TemplateRegistry<ITemplate>();
            var sparkSet = new SparkEngineSettings().Search;

            _sharingGraph = new SharingGraph();
            _sharingGraph.Global("Global");
            _sharingGraph.CompileDependencies("Package1", "Package2");

            new ContentFolder(TemplateConstants.HostOrigin, pathApp).FindFiles(sparkSet)
                .Union(new ContentFolder("Package1", pathPackage1).FindFiles(sparkSet)
                .Union(new ContentFolder("Package2", pathPackage2).FindFiles(sparkSet)
                .Union(new ContentFolder("Global", globalPackage).FindFiles(sparkSet))))
                .Each(x =>
                {
                    if (x.Provenance == TemplateConstants.HostOrigin && x.Path.StartsWith(pathPackage1)) return;
                    templateRegistry.Register(new Template(x.Path, x.ProvenancePath, x.Provenance));
                });

            var viewPathPolicy = new ViewPathPolicy<ITemplate>();
            templateRegistry.Each(viewPathPolicy.Apply);

            _viewFolder = new TemplateViewFolder(templateRegistry);
            _templateDirectoryProvider = new TemplateDirectoryProvider<ITemplate>(new SharedPathBuilder(), templateRegistry, _sharingGraph);
            _engine = new SparkViewEngine
            {
                ViewFolder = _viewFolder,
                BindingProvider = new FubuBindingProvider(new SparkTemplateRegistry(templateRegistry)),
                PartialProvider = new FubuPartialProvider(_templateDirectoryProvider)
            };

            _pak1TemplateRegistry = new TemplateRegistry<ITemplate>(templateRegistry.ByOrigin(Package1));
            _pak2TemplateRegistry = new TemplateRegistry<ITemplate>(templateRegistry.ByOrigin(Package2));
            _appTemplateRegistry = new TemplateRegistry<ITemplate>(templateRegistry.FromHost());
            _globalTemplateRegistry = new TemplateRegistry<ITemplate>(templateRegistry.ByOrigin(Global));
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
            _appTemplateRegistry.ShouldHaveCount(11);
            _pak1TemplateRegistry.ShouldHaveCount(13);
            _pak2TemplateRegistry.ShouldHaveCount(8);
            _globalTemplateRegistry.ShouldHaveCount(2);
        }

        [Test]
        public void views_with_same_path_are_resolved_correctly()
        {
            var hostView = _appTemplateRegistry.FirstByName("_samePath");
            var pak1View = _pak1TemplateRegistry.FirstByName("_samePath");
            var pak2View = _pak2TemplateRegistry.FirstByName("_samePath");

            getViewSource(hostView).ShouldEqual("Host _samePath.spark");
            getViewSource(pak1View).ShouldEqual("Package1 _samePath.spark");
            getViewSource(pak2View).ShouldEqual("Package2 _samePath.spark");
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

            getViewSource(threeView).ShouldEqual("<header/> MacPro");
            renderTemplate(threeView).ShouldEqual("This is the header MacPro");
        }

        [Test]
        public void host_views_are_isolated_from_packages()
        {
            var noLuck = _appTemplateRegistry.FirstByName("NoLuck");
            getViewSource(noLuck).ShouldEqual("Will <fail/>");
            renderTemplate(noLuck).ShouldEqual("Will <fail></fail>");
        }

        [Test]
        public void views_from_packages_are_isolated_among_packages()
        {
            var dosView = _pak1TemplateRegistry.FirstByName("SerieT");
            var dueView = _pak2TemplateRegistry.FirstByName("Inspiron");

            getViewSource(dosView).ShouldEqual("SerieT <dell/>");
            renderTemplate(dosView).ShouldEqual("SerieT <dell></dell>");

            getViewSource(dueView).ShouldEqual("Inspiron <lenovo/>");
            renderTemplate(dueView).ShouldEqual("Inspiron <lenovo></lenovo>");
        }

        [Test]
        public void view_from_packages_can_refer_global_partials_in_shared_paths()
        {
            var view = _pak1TemplateRegistry.FirstByName("SerieG");
            getViewSource(view).ShouldEqual("SerieT <globalPartial/>");
            renderTemplate(view).ShouldEqual("SerieT Global shared partial");
        }

        [Test]
        public void view_from_packages_can_not_refer_global_partials_from_paths_other_than_shared_paths()
        {
            var view = _pak1TemplateRegistry.FirstByName("SerieGx");
            getViewSource(view).ShouldEqual("SerieT <notShared/>");
            renderTemplate(view).ShouldEqual("SerieT <notShared></notShared>");
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

            getViewSource(cuatroView).ShouldEqual("<use master=\"Maker\"/> SerieX");
            renderTemplate(cuatroView, master).ShouldEqual("Lenovo SerieX");
        }

        [Test]
        public void binding_works_under_package_context()
        {
            var serieZ = _pak1TemplateRegistry.FirstByName("SerieZ");
            serieZ.Descriptor = new SparkDescriptor(serieZ, new SparkViewEngine());
            serieZ.Descriptor.As<SparkDescriptor>().AddBinding(_pak1TemplateRegistry.First(x => x.Name() == "bindings"));
            serieZ.Descriptor.As<SparkDescriptor>().AddBinding(_appTemplateRegistry.First(x => x.Name() == "bindings"));
            renderTemplate(serieZ).ShouldEqual("SerieZ Hi from Package1 Bye from Host");
        }

        [Test]
        public void binding_works_under_host_context()
        {
            var macMini = _appTemplateRegistry.FirstByName("MacMini");
            macMini.Descriptor = new SparkDescriptor(macMini, new SparkViewEngine());
            macMini.Descriptor.As<SparkDescriptor>().AddBinding(_appTemplateRegistry.First(x => x.Name() == "bindings"));
            var content = renderTemplate(macMini);
            content.ShouldEqual(@"MacMini Hi from Host");
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