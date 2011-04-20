using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Tokenization;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkItemViewFolderTests
{
    [TestFixture]
    public class BasicTester
    {
        private readonly string _hostRoot;
        private readonly string _pak1;
        private readonly string _pak2;

        private readonly SparkItem _hostHomeView;
        private readonly SparkItem _hostApplicationView;
        private readonly SparkItem _hostFooterPartialView;
        private readonly SparkItem _pak1HomeView;
        private readonly SparkItem _pak1NamePartialView;
        private readonly SparkItem _pak2HomeView;
        private readonly SparkItem _pak2ApplicationView;
        private readonly SparkItem _pak2ThemeView;

        private readonly SparkItemViewFolder _viewFolder;

        public BasicTester()
        {
            _hostRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            _pak1 = Path.Combine(_hostRoot, "Pak1");
            _pak2 = Path.Combine(_hostRoot, "Pak2");

            _hostHomeView = new SparkItem(Path.Combine(_hostRoot, "Home", "Home.spark"), _hostRoot, Constants.HostOrigin);
            _hostApplicationView = new SparkItem(Path.Combine(_hostRoot, "Shared", "application.spark"), _hostRoot, Constants.HostOrigin);
            _hostFooterPartialView = new SparkItem(Path.Combine(_hostRoot, "Shared", "_footer.spark"), _hostRoot, Constants.HostOrigin);

            _pak1HomeView = new SparkItem(Path.Combine(_pak1, "Home", "Home.spark"), _hostRoot, "Pak1");
            _pak1NamePartialView = new SparkItem(Path.Combine(_pak1, "Home", "_name.spark"), _hostRoot, "Pak1");

            _pak2HomeView = new SparkItem(Path.Combine(_pak2, "Home", "Home.spark"), _hostRoot, "Pak2");
            _pak2ApplicationView = new SparkItem(Path.Combine(_pak2, "Shared", "application.spark"), _hostRoot, "Pak2");
            _pak2ThemeView = new SparkItem(Path.Combine(_pak2, "Shared", "theme.spark"), _hostRoot, "Pak2");

            var sparkItems = new List<SparkItem>
            {
                _hostHomeView, _hostApplicationView, _hostFooterPartialView,
                _pak1HomeView, _pak1NamePartialView,
                _pak2HomeView, _pak2ApplicationView, _pak2ThemeView
            };

            _viewFolder = new SparkItemViewFolder(sparkItems);
        }

        [Test]
        public void can_resolve_view_from_host()
        {
            _viewFolder.HasView(_hostHomeView.RelativePath())
                .ShouldBeTrue();
        }

        [Test]
        public void can_resolve_master_from_host()
        {
            _viewFolder.HasView(_hostApplicationView.RelativePath())
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

        [Test]
        public void listviews_returns_views_in_prefixed_relative_path()
        {
            var foundViews = new List<string>();
            
            _hostHomeView
                .PrefixedRelativePath().getPathParts()
                .Union(new[] { Constants.SharedSpark })
                .Each(path => foundViews.AddRange(_viewFolder.ListViews(path)));
                
            foundViews.ShouldHaveTheSameElementsAs(
                _hostHomeView.PrefixedRelativePath(),
                _hostApplicationView.PrefixedRelativePath(),
                _hostFooterPartialView.PrefixedRelativePath());
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
                .ShouldEqual(@"home from <name /><footer />");
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
            readfromStream(_hostHomeView.RelativePath())
                .ShouldEqual(@"home from Root");
        }

        [Test]
        public void returns_viewsource_for_master_from_host()
        {
            readfromStream(_hostApplicationView.RelativePath())
                .ShouldEqual(@"<div>Host Application: <use:view/></div>");
        }

        private string readfromStream(string path)
        {
            using (var stream = _viewFolder.GetViewSource(path).OpenViewStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
