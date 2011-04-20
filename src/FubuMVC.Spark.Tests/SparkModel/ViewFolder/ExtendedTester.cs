using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;
using System.Collections.Generic;
using Spark;
using Constants = FubuMVC.Spark.SparkModel.Constants;

namespace FubuMVC.Spark.Tests.SparkModel.ViewFolder
{
    [TestFixture]
    public class ExtendedTester
    {
        private const string Package1 = "Package1";
        private const string Package2 = "Package2";

        private readonly SparkItemViewFolder _viewFolder;
        private readonly ISparkViewEngine _engine;

        private readonly SparkItems _pak1Items;
        private readonly SparkItems _pak2Items;
        private readonly SparkItems _appItems;

        public ExtendedTester()
        {
            var testRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

            var pathApp = Path.Combine(testRoot, "App");
            var pathPackage1 = Path.Combine(pathApp, "Content", "Package1", "WebContent");
            var pathPackage2 = Path.Combine(testRoot, "Package2");

            var roots = new List<SparkRoot>
            {
                new SparkRoot {Origin = Package1, Path = pathPackage1},
                new SparkRoot {Origin = Package2, Path = pathPackage2},
                new SparkRoot {Origin = Constants.HostOrigin, Path = pathApp}
            };
            
            var scanner = new SparkItemFinder(new FileScanner(), roots);
            var allItems = new SparkItems(scanner.FindItems());

            _viewFolder = new SparkItemViewFolder(allItems);
            _engine = new SparkViewEngine { ViewFolder = _viewFolder };

            _pak1Items = new SparkItems(allItems.Where(x => x.Origin == Package1));
            _pak2Items = new SparkItems(allItems.Where(x => x.Origin == Package2));
            _appItems = new SparkItems(allItems.Where(x => x.Origin == Constants.HostOrigin));
        }

        [Test]
        public void host_views_are_located_correctly()
        {
            var one = _appItems.FirstByName("MacBook");
            var footer = _appItems.FirstByName("_footer");

            getViewSource(one).ShouldEqual("MacBook");
            getViewSource(footer).ShouldEqual("This is the footer");
        }


        [Test]
        public void deployed_package_views_are_located_correctly()
        {
            var uno = _pak1Items.FirstByName("SerieSL");
            var header = _pak1Items.FirstByName("_header");

            getViewSource(uno).ShouldEqual("<appname/> SerieSL");
            getViewSource(header).ShouldEqual("Lenovo Header");
        }


        [Test]
        public void dev_package_views_are_located_correctly()
        {
            var uno = _pak2Items.FirstByName("Vostro");
            var header = _pak2Items.FirstByName("_footer");

            getViewSource(uno).ShouldEqual("<appname/> Vostro");
            getViewSource(header).ShouldEqual("Dell footer");
        }

        [Test]
        public void views_with_same_path_are_resolved_correctly()
        {
            var hostView = _appItems.FirstByName("_samePath");
            var pak1View = _pak1Items.FirstByName("_samePath");
            var pak2View = _pak2Items.FirstByName("_samePath");

            getViewSource(hostView).ShouldEqual("Host _samePath.spark");
            getViewSource(pak1View).ShouldEqual("Package1 _samePath.spark");
            getViewSource(pak2View).ShouldEqual("Package2 _samePath.spark");
        }

        [Test]
        public void views_from_packages_can_refer_to_other_views_from_the_same_package()
        {
            var tresView = _pak1Items.FirstByName("SerieW");
            var treView = _pak2Items.FirstByName("Xps");

            getViewSource(tresView).ShouldEqual("<header/> SerieW");
            renderSparkItem(tresView).ShouldEqual("Lenovo Header SerieW");
            
            getViewSource(treView).ShouldEqual("Xps <footer/>");
            renderSparkItem(treView).ShouldEqual("Xps Dell footer");
        }


        [Test]
        public void views_from_host_can_refer_to_other_views_from_the_host()
        {
            var threeView = _appItems.FirstByName("MacPro");

            getViewSource(threeView).ShouldEqual("<header/> MacPro");
            renderSparkItem(threeView).ShouldEqual("This is the header MacPro");
        }

        [Test]
        public void host_views_are_isolated_from_packages()
        {
            var noLuck = _appItems.FirstByName("NoLuck");
            getViewSource(noLuck).ShouldEqual("Will <fail/>");
            renderSparkItem(noLuck).ShouldEqual("Will <fail/>");
        }

        [Test]
        public void views_from_packages_are_isolated_among_packages()
        {
            var dosView = _pak1Items.FirstByName("SerieT");
            var dueView = _pak2Items.FirstByName("Inspiron");

            getViewSource(dosView).ShouldEqual("SerieT <dell/>");
            renderSparkItem(dosView).ShouldEqual("SerieT <dell/>");
            
            getViewSource(dueView).ShouldEqual("Inspiron <lenovo/>");
            renderSparkItem(dueView).ShouldEqual("Inspiron <lenovo/>");
        }

        [Test]
        public void views_from_packages_can_refer_views_from_top_level_shared_directory_in_host()
        {
            var pak1UnoView = _pak1Items.FirstByName("SerieSL");
            var pak2UnoView = _pak2Items.FirstByName("Vostro");

            getViewSource(pak1UnoView).ShouldEqual("<appname/> SerieSL");
            renderSparkItem(pak1UnoView).ShouldEqual("Computers Catalog SerieSL");

            getViewSource(pak2UnoView).ShouldEqual("<appname/> Vostro");
            renderSparkItem(pak2UnoView).ShouldEqual("Computers Catalog Vostro");
        }

        [Test]
        public void views_from_packages_can_use_masters_from_the_same_package()
        {
            var nl = Environment.NewLine;
            var cuatroView = _pak1Items.FirstByName("SerieX");
            var master = _pak1Items.FirstByName("Maker");

            getViewSource(cuatroView).ShouldEqual("<use master=\"Maker\"/>{0}SerieX".ToFormat(nl));           
            renderSparkItem(cuatroView, master).ShouldEqual("Lenovo{0}SerieX".ToFormat(nl));
        }

        private string getViewSource(SparkItem item)
        {
            var content = _viewFolder.GetViewSource(item.PrefixedRelativePath());
            using (var stream = content.OpenViewStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string renderSparkItem(SparkItem item, params SparkItem[] templates)
        {
            templates = templates ?? Enumerable.Empty<SparkItem>().ToArray();
            var descriptor = new SparkViewDescriptor();
            descriptor.AddTemplate(item.PrefixedRelativePath());
            templates.Each(x => descriptor.AddTemplate(x.PrefixedRelativePath()));

            var instance = _engine.CreateInstance(descriptor);
            var writer = new StringWriter();
            instance.RenderView(writer);
            return writer.ToString();
        }
    }
}