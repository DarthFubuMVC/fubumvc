using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

// TODO: Convert this to integration tests

namespace FubuMVC.Razor.Tests.RazorModel
{
    [TestFixture]
    public class ExtendedTester
    {
        private const string Package1 = "Package1";
        private const string Package2 = "Package2";

        private readonly TemplateRegistry<IRazorTemplate> _pak1TemplateRegistry;
        private readonly TemplateRegistry<IRazorTemplate> _pak2TemplateRegistry;
        private readonly TemplateRegistry<IRazorTemplate> _appTemplateRegistry;

        private readonly ITemplateFactory _templateFactory;

        private readonly IServiceLocator _serviceLocator;
        private readonly ISharedTemplateLocator<IRazorTemplate> _sharedTemplateLocator;

        public ExtendedTester()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var pathApp = Path.Combine(testRoot, "App");
            var pathPackage1 = Path.Combine(pathApp, "fubu-packages", "Package1", "WebContent");
            var pathPackage2 = Path.Combine(testRoot, "Package2");

            var allTemplates = new TemplateRegistry<IRazorTemplate>();

            var razorSettings = new RazorEngineSettings();
            var razorSet = razorSettings.Search;

            new ContentFolder(TemplateConstants.HostOrigin, pathApp).FindFiles(razorSet)
                .Union(new ContentFolder("Package1", pathPackage1).FindFiles(razorSet)
                .Union(new ContentFolder("Package2", pathPackage2).FindFiles(razorSet)))
                .Each(x =>
                {
                    if (x.Provenance == TemplateConstants.HostOrigin && x.Path.StartsWith(pathPackage1)) return;
                    allTemplates.Register(new Template(x.Path, x.ProvenancePath, x.Provenance));
                });

            var viewPathPolicy = new ViewPathPolicy<IRazorTemplate>();
            allTemplates.Each(x =>
            {
                viewPathPolicy.Apply(x);
                x.Descriptor = new ViewDescriptor<IRazorTemplate>(x);
            });

            var commonNamespaces = new CommonViewNamespaces();
            commonNamespaces.AddForType<string>();
            _templateFactory = new TemplateFactoryCache(commonNamespaces, razorSettings, new TemplateCompiler(), new RazorTemplateGenerator());

            _pak1TemplateRegistry = new TemplateRegistry<IRazorTemplate>(allTemplates.ByOrigin(Package1));
            _pak2TemplateRegistry = new TemplateRegistry<IRazorTemplate>(allTemplates.ByOrigin(Package2));
            _appTemplateRegistry = new TemplateRegistry<IRazorTemplate>(allTemplates.FromHost());

            _serviceLocator = MockRepository.GenerateMock<IServiceLocator>();

            var sharingGraph = new SharingGraph();
            sharingGraph.Dependency("Package1", ContentFolder.Application);
            sharingGraph.Dependency("Package2", ContentFolder.Application);
            var templateDirectory = new TemplateDirectoryProvider<IRazorTemplate>(new SharedPathBuilder(), allTemplates,
                                                                                  sharingGraph);
            _sharedTemplateLocator = new SharedTemplateLocator<IRazorTemplate>(templateDirectory, allTemplates,
                                                                               new RazorTemplateSelector());

            var partialRenderer = new PartialRenderer(_sharedTemplateLocator, _templateFactory);
            _serviceLocator.Expect(x => x.GetInstance(Arg.Is(typeof (IPartialRenderer)))).Return(partialRenderer);
            _serviceLocator.Expect(x => x.GetInstance(typeof (IOutputWriter))).Return(MockRepository.GenerateMock<IOutputWriter>());
        }

        private string getViewSource(IRazorTemplate template)
        {
            using (var stream = new FileStream(template.FilePath, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string renderTemplate(IRazorTemplate template, params IRazorTemplate[] templates)
        {
            var descriptor = new ViewDescriptor<IRazorTemplate>(template);
            var current = descriptor;
            for (var i = 0; i < templates.Length; ++i)
            {
                var layoutTemplate = templates[i];
                var layout = new ViewDescriptor<IRazorTemplate>(layoutTemplate);
                layoutTemplate.Descriptor = layout;
                current.Master = templates[i];
                current = layout;
            }



            var viewFactory = new RazorViewToken(descriptor, _templateFactory);
            var view = (IFubuRazorView)viewFactory.GetView();
            view.ServiceLocator = _serviceLocator;
            view.As<IRenderableView>().Render();
            return view.Result.ToString();
        }

        [Test]
        public void deployed_package_views_are_located_correctly()
        {
            var uno = _pak1TemplateRegistry.FirstByName("SerieSL");
            var header = _pak1TemplateRegistry.FirstByName("_header");

            getViewSource(uno).ShouldEqual("@this.RenderPartial(\"appname\") SerieSL");
            getViewSource(header).ShouldEqual("Lenovo Header");
        }

        [Test]
        public void dev_package_views_are_located_correctly()
        {
            var uno = _pak2TemplateRegistry.FirstByName("Vostro");
            var header = _pak2TemplateRegistry.FirstByName("_footer");

            getViewSource(uno).ShouldEqual("@this.RenderPartial(\"appname\") Vostro");
            getViewSource(header).ShouldEqual("Dell footer");
        }

        [Test]
        public void null_attribute_values_do_not_get_rendered()
        {
            var attributeSample = _pak2TemplateRegistry.FirstByName("AttributeSample");

            getViewSource(attributeSample).ShouldEqual("<test myattribute=\"@null\"></test>");
            renderTemplate(attributeSample).ShouldEqual("<test></test>");
        }

        [Test]
        public void host_views_are_isolated_from_packages()
        {
            var noLuck = _appTemplateRegistry.FirstByName("NoLuck");
            getViewSource(noLuck).ShouldEqual("Will @this.RenderPartial(\"fail\")");
            Exception<NullReferenceException>.ShouldBeThrownBy(() => renderTemplate(noLuck));
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
        public void the_correct_number_of_templates_are_resolved()
        {
            _appTemplateRegistry.ShouldHaveCount(10);
            _pak1TemplateRegistry.ShouldHaveCount(10);
            _pak2TemplateRegistry.ShouldHaveCount(9);
        }

        [Test]
        public void views_from_host_can_refer_to_other_views_from_the_host()
        {
            var threeView = _appTemplateRegistry.FirstByName("MacPro");

            getViewSource(threeView).ShouldEqual("@this.RenderPartial(\"header\") <p>MacPro</p>");
            renderTemplate(threeView).ShouldEqual("This is the header <p>MacPro</p>");
        }

        [Test]
        public void views_from_packages_are_isolated_among_packages()
        {
            var dosView = _pak1TemplateRegistry.FirstByName("SerieT");
            var dueView = _pak2TemplateRegistry.FirstByName("Inspiron");

            getViewSource(dosView).ShouldEqual("SerieT @this.RenderPartial(\"dell\")");
            Exception<NullReferenceException>.ShouldBeThrownBy(() => renderTemplate(dosView));

            getViewSource(dueView).ShouldEqual("Inspiron @this.RenderPartial(\"lenovo\")");
            Exception<NullReferenceException>.ShouldBeThrownBy(() => renderTemplate(dueView));
        }

        [Test]
        public void views_from_packages_can_refer_to_other_views_from_the_same_package()
        {
            var tresView = _pak1TemplateRegistry.FirstByName("SerieW");
            var treView = _pak2TemplateRegistry.FirstByName("Xps");

            getViewSource(tresView).ShouldEqual("@this.RenderPartial(\"header\") SerieW");
            renderTemplate(tresView).ShouldEqual("Lenovo Header SerieW");

            getViewSource(treView).ShouldEqual("Xps @this.RenderPartial(\"footer\")");
            renderTemplate(treView).ShouldEqual("Xps Dell footer");
        }

        [Test]
        public void views_from_packages_can_refer_views_from_top_level_shared_directory_in_host()
        {
            var pak1UnoView = _pak1TemplateRegistry.FirstByName("SerieSL");
            var pak2UnoView = _pak2TemplateRegistry.FirstByName("Vostro");

            getViewSource(pak1UnoView).ShouldEqual("@this.RenderPartial(\"appname\") SerieSL");
            renderTemplate(pak1UnoView).ShouldEqual("Computers Catalog SerieSL");

            getViewSource(pak2UnoView).ShouldEqual("@this.RenderPartial(\"appname\") Vostro");
            renderTemplate(pak2UnoView).ShouldEqual("Computers Catalog Vostro");
        }

        [Test]
        public void views_from_packages_can_use_masters_from_the_same_package()
        {
            var cuatroView = _pak1TemplateRegistry.FirstByName("SerieX");
            var master = _pak1TemplateRegistry.FirstByName("Maker");

            getViewSource(cuatroView).ShouldEqual("@section sectiontest { from section } SerieX @layout Maker");
            var template = renderTemplate(cuatroView, master);
            template.ShouldEqual("Lenovo  SerieX   from section ");
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
    }
}