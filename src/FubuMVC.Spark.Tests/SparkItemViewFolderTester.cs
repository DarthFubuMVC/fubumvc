using System.Collections.Generic;
using System.IO;
using FubuMVC.Spark.Tokenization;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkItemViewFolderTester
    {
        private readonly string _hostRoot;
        private readonly string _pak1;
        private readonly string _pak2;

        private readonly SparkItem _hostHomeView;
        private readonly SparkItem _hostApplicationView;
        private readonly SparkItem _pak1HomeView;
        private readonly SparkItem _pak1NamePartialView;
        private readonly SparkItem _pak2HomeView;
        private readonly SparkItem _pak2ApplicationView;
        private readonly SparkItem _pak2ThemeView;

        private readonly SparkItemViewFolder _viewFolder;

        public SparkItemViewFolderTester()
        {
            _hostRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            _pak1 = Path.Combine(_hostRoot, "Pak1");
            _pak2 = Path.Combine(_hostRoot, "Pak2");

            _hostHomeView = new SparkItem(Path.Combine(_hostRoot, "Home", "Home.spark"), _hostRoot, Constants.HostOrigin);
            _hostApplicationView = new SparkItem(Path.Combine(_hostRoot, "Shared", "application.spark"), _hostRoot, Constants.HostOrigin);

            _pak1HomeView = new SparkItem(Path.Combine(_pak1, "Home", "Home.spark"), _hostRoot, "Pak1");
            _pak1NamePartialView = new SparkItem(Path.Combine(_pak1, "Home", "_name.spark"), _hostRoot, "Pak1");

            _pak2HomeView = new SparkItem(Path.Combine(_pak2, "Home", "Home.spark"), _hostRoot, "Pak2");
            _pak2ApplicationView = new SparkItem(Path.Combine(_pak2, "Shared", "application.spark"), _hostRoot, "Pak2");
            _pak2ThemeView = new SparkItem(Path.Combine(_pak2, "Shared", "theme.spark"), _hostRoot, "Pak2");

            var sparkItems = new List<SparkItem>
            {
                _hostHomeView,
                _hostApplicationView,
                _pak1HomeView,
                _pak1NamePartialView,
                _pak2HomeView,
                _pak2ApplicationView,
                _pak2ThemeView
            };

            _viewFolder = new SparkItemViewFolder(sparkItems);
        }

        [Test]
        public void can_resolve_view_from_host()
        {
            _viewFolder.HasView(_hostHomeView.PrefixedRelativePath())
                .ShouldBeTrue();
        }

        [Test]
        public void can_resolve_master_from_host()
        {
            _viewFolder.HasView(_hostApplicationView.PrefixedRelativePath())
                .ShouldBeTrue();
        }

        [Test]
        public void can_resolve_view_from_package()
        {
            _viewFolder.HasView(_pak1HomeView.PrefixedRelativePath())
                .ShouldBeTrue();
        }

        [Test]
        public void can_resolve_master_from_package()
        {
            _viewFolder.HasView(_pak2ApplicationView.PrefixedRelativePath())
                .ShouldBeTrue();
        }

        [Test]
        public void can_resolve_partial_from_package()
        {
            _viewFolder.HasView(_pak1NamePartialView.PrefixedRelativePath())
                .ShouldBeTrue();
        }

        [Test, Ignore("Shouldn't we rather return by origin?")]
        public void listviews_returns_views_by_origin()
        {
            // ListViews is used by FindPartialFiles in Spark.Parser.ViewLoader when it
            // tries to locate partials files in ancestor path.
            // ?? PrefixedVirtualDirectoryPath ?? 
            _viewFolder.ListViews(_hostHomeView.Origin)
                .ShouldHaveTheSameElementsAs(_hostHomeView.PrefixedRelativePath(), 
                                             _hostApplicationView.PrefixedRelativePath());
        }

        [Test]
        public void returns_viewsource_for_partial_from_package()
        {
            readfromStream(_pak1NamePartialView.PrefixedRelativePath())
                .ShouldEqual("Pak1");
        }

        [Test]
        public void returns_viewsource_for_view_from_package()
        {
            readfromStream(_pak1HomeView.PrefixedRelativePath())
                .ShouldEqual(@"home from <name />");
        }

        [Test]
        public void returns_viewsource_for_master_from_package()
        {
            readfromStream(_pak2ApplicationView.PrefixedRelativePath())
                .ShouldEqual(@"<div>Pak2 Application: <use:view/></div>");
        }

        [Test]
        public void returns_viewsource_for_view_from_host()
        {
            readfromStream(_hostHomeView.PrefixedRelativePath())
                .ShouldEqual(@"home from Root");
        }

        [Test]
        public void returns_viewsource_for_master_from_host()
        {
            readfromStream(_hostApplicationView.PrefixedRelativePath())
                .ShouldEqual(@"<div>Host Application: <use:view/></div>");
        }

        private string readfromStream(string path)
        {
            var stream =_viewFolder.GetViewSource(path).OpenViewStream();
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
