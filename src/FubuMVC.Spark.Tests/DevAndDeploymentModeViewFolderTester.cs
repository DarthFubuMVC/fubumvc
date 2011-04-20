using System;
using System.IO;
using System.Linq;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Scanning;
using NUnit.Framework;
using System.Collections.Generic;
using Spark;
using Constants = FubuMVC.Spark.Tokenization.Constants;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class DevAndDeploymentModeViewFolderTester
    {
        private readonly string _testRoot;
        private readonly string _hostRoot;
        private readonly string _pathPackage1;
        private readonly string _pathPackage2;

        private const string Package1 = "Package1";
        private const string Package2 = "Package2";

        private readonly SparkItemViewFolder _viewFolder;
        private readonly ISparkViewEngine _engine;
        private readonly List<SparkItem> _items;

        public DevAndDeploymentModeViewFolderTester()
        {
            _testRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            _hostRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "App");
            _pathPackage1 = Path.Combine(_hostRoot, "Content", "Package1", "WebContent");
            _pathPackage2 = Path.Combine(_testRoot, "Package2");

            _items = new List<SparkItem>();
            var roots = new List<SparkRoot>
                            {
                                new SparkRoot {Origin = Package1, Path = _pathPackage1},
                                new SparkRoot {Origin = Package2, Path = _pathPackage2},
                                new SparkRoot {Origin = Constants.HostOrigin, Path = _hostRoot}
                            };
            var scanner = new SparkItemFinder(new FileScanner(), roots);
            _items.AddRange(scanner.FindItems());


            _viewFolder = new SparkItemViewFolder(_items);
            _engine = new SparkViewEngine { ViewFolder = _viewFolder };
        }

        [Test]
        public void host_views_are_located_correctly()
        {
            var one = _items.Where(x => x.Origin == Constants.HostOrigin)
                .Where(x => x.Name() == "MacBook").First();
            var footer = _items.Where(x => x.Origin == Constants.HostOrigin)
                .Where(x => x.Name() == "_footer").First();

            var sourceOne = getViewSource(one);
            var sourceFooter = getViewSource(footer);

            Assert.AreEqual("MacBook", sourceOne);
            Assert.AreEqual("This is the footer", sourceFooter);
        }


        [Test]
        public void deployed_package_views_are_located_correctly()
        {
            var uno = _items.Where(x => x.Origin == Package1)
                .Where(x => x.Name() == "SerieSL").First();
            var header = _items.Where(x => x.Origin == Package1)
                .Where(x => x.Name() == "_header").First();

            var sourceUno = getViewSource(uno);
            var sourceHeader = getViewSource(header);

            Assert.AreEqual("<appname/> SerieSL", sourceUno);
            Assert.AreEqual("Lenovo Header", sourceHeader);
        }


        [Test]
        public void dev_package_views_are_located_correctly()
        {
            var uno = _items.Where(x => x.Origin == Package2)
                .Where(x => x.Name() == "Vostro").First();
            var header = _items.Where(x => x.Origin == Package2)
                .Where(x => x.Name() == "_footer").First();

            var sourceUno = getViewSource(uno);
            var sourceHeader = getViewSource(header);

            Assert.AreEqual("<appname/> Vostro", sourceUno);
            Assert.AreEqual("Dell footer", sourceHeader);
        }

        [Test]
        public void views_with_same_path_are_resolved_correctly()
        {
            var hostView = _items.Where(x => x.Origin == Constants.HostOrigin)
                .Where(x => x.Name() == "_samePath").First();
            var pak1View = _items.Where(x => x.Origin == Package1)
                .Where(x => x.Name() == "_samePath").First();
            var pak2View = _items.Where(x => x.Origin == Package2)
                .Where(x => x.Name() == "_samePath").First();

            var sourceHostView = getViewSource(hostView);
            var sourcePak1View = getViewSource(pak1View);
            var sourcePak2View = getViewSource(pak2View);

            Assert.AreEqual("Host _samePath.spark", sourceHostView);
            Assert.AreEqual("Package1 _samePath.spark", sourcePak1View);
            Assert.AreEqual("Package2 _samePath.spark", sourcePak2View);
        }

        [Test]
        public void views_from_packages_can_refer_to_other_views_from_the_same_package()
        {
            var tresView = _items
                .Where(x => x.Origin == Package1).First(x => x.Name() == "SerieW");
            var treView = _items
                .Where(x => x.Origin == Package2).First(x => x.Name() == "Xps");

            var sourceTresView = getViewSource(tresView);
            var contentTresView = renderSparkItem(tresView);
            var sourceTreView = getViewSource(treView);
            var contentTreView = renderSparkItem(treView);

            Assert.AreEqual("<header/> SerieW", sourceTresView);
            Assert.AreEqual("Lenovo Header SerieW", contentTresView);

            Assert.AreEqual("Xps <footer/>", sourceTreView);
            Assert.AreEqual("Xps Dell footer", contentTreView);
        }


        [Test]
        public void views_from_host_can_refer_to_other_views_from_the_same_host()
        {
            var threeView = _items
                .Where(x => x.Origin == Constants.HostOrigin).First(x => x.Name() == "MacPro");

            var sourceThreeView = getViewSource(threeView);
            var contentThreeView = renderSparkItem(threeView);

            Assert.AreEqual("<header/> MacPro", sourceThreeView);
            Assert.AreEqual("This is the header MacPro", contentThreeView);
        }

        [Test]
        public void views_from_packages_are_isolated()
        {
            var dosView = _items
                .Where(x => x.Origin == Package1).First(x => x.Name() == "SerieT");
            var dueView = _items
                .Where(x => x.Origin == Package2).First(x => x.Name() == "Inspiron");

            var sourceDosView = getViewSource(dosView);
            var contentDosView = renderSparkItem(dosView);
            var sourceDueView = getViewSource(dueView);
            var contentDueView = renderSparkItem(dueView);

            Assert.AreEqual("SerieT <dell/>", sourceDosView);
            Assert.AreEqual("SerieT <dell/>", contentDosView);

            Assert.AreEqual("Inspiron <lenovo/>", sourceDueView);
            Assert.AreEqual("Inspiron <lenovo/>", contentDueView);
        }

        [Test]
        public void views_from_packages_can_refer_views_from_top_level_shared_directory_in_host()
        {
            var pak1UnoView = _items
                .Where(x => x.Origin == Package1).First(x => x.Name() == "SerieSL");
            var pak2UnoView = _items
                .Where(x => x.Origin == Package2).First(x => x.Name() == "Vostro");

            var sourcePak1UnoView = getViewSource(pak1UnoView);
            var contentPak1UnoView = renderSparkItem(pak1UnoView);
            var sourcePak2UnoView = getViewSource(pak2UnoView);
            var contentPak2UnoView = renderSparkItem(pak2UnoView);

            Assert.AreEqual("<appname/> SerieSL", sourcePak1UnoView);
            Assert.AreEqual("Computers Catalog SerieSL", contentPak1UnoView);

            Assert.AreEqual("<appname/> Vostro", sourcePak2UnoView);
            Assert.AreEqual("Computers Catalog Vostro", contentPak2UnoView);
        }

        [Test]
        public void views_from_packages_can_use_masters_from_the_same_package()
        {
            var cuatroView = _items
                .Where(x => x.Origin == Package1).First(x => x.Name() == "SerieX");
            var master = _items
                .Where(x => x.Origin == Package1).First(x => x.Name() == "Maker");
            var sourceCuatroView = getViewSource(cuatroView);
            var contentCuatroView = renderSparkItem(cuatroView, master);

            Assert.AreEqual("<use master=\"Maker\"/>" + Environment.NewLine + "SerieX", sourceCuatroView);
            Assert.AreEqual("Lenovo" + Environment.NewLine + "SerieX", contentCuatroView);
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