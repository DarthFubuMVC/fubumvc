using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel.ViewFolder
{
    [TestFixture]
    public class BasicTester
    {
        private readonly string _hostRoot;
        private readonly string _pak1;
        private readonly string _pak2;

        private readonly ITemplate _hostHomeView;
        private readonly ITemplate _hostApplicationView;
        private readonly ITemplate _hostFooterPartialView;
        private readonly ITemplate _pak1HomeView;
        private readonly ITemplate _pak1NamePartialView;
        private readonly ITemplate _pak2HomeView;
        private readonly ITemplate _pak2ApplicationView;
        private readonly ITemplate _pak2ThemeView;

        private readonly TemplateViewFolder _viewFolder;

        public BasicTester()
        {
            _hostRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            _pak1 = Path.Combine(_hostRoot, "Pak1");
            _pak2 = Path.Combine(_hostRoot, "Pak2");

            _hostHomeView = new Template(Path.Combine(_hostRoot, "Home", "Home.spark"), _hostRoot, TemplateConstants.HostOrigin);
            _hostApplicationView = new Template(Path.Combine(_hostRoot, "Shared", "application.spark"), _hostRoot, TemplateConstants.HostOrigin);
            _hostFooterPartialView = new Template(Path.Combine(_hostRoot, "Shared", "_footer.spark"), _hostRoot, TemplateConstants.HostOrigin);

            _pak1HomeView = new Template(Path.Combine(_pak1, "Home", "Home.spark"), _hostRoot, "Pak1");
            _pak1NamePartialView = new Template(Path.Combine(_pak1, "Home", "_name.spark"), _hostRoot, "Pak1");

            _pak2HomeView = new Template(Path.Combine(_pak2, "Home", "Home.spark"), _hostRoot, "Pak2");
            _pak2ApplicationView = new Template(Path.Combine(_pak2, "Shared", "application.spark"), _hostRoot, "Pak2");
            _pak2ThemeView = new Template(Path.Combine(_pak2, "Shared", "theme.spark"), _hostRoot, "Pak2");

            var templates = new List<ITemplate>
            {
                _hostHomeView, _hostApplicationView, _hostFooterPartialView,
                _pak1HomeView, _pak1NamePartialView,
                _pak2HomeView, _pak2ApplicationView, _pak2ThemeView
            };

            var viewPathPolicy = new ViewPathPolicy<ITemplate>();
            templates.Each(viewPathPolicy.Apply);

            _viewFolder = new TemplateViewFolder(new TemplateRegistry<ITemplate>(templates));
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
            _viewFolder.HasView(_pak1HomeView.ViewPath)
                .ShouldBeTrue();
        }

        [Test]
        public void can_resolve_master_from_package()
        {
            _viewFolder.HasView(_pak2ApplicationView.ViewPath)
                .ShouldBeTrue();
        }

        [Test]
        public void can_resolve_partial_from_package()
        {
            _viewFolder.HasView(_pak1NamePartialView.ViewPath)
                .ShouldBeTrue();
        }

        [Test]
        public void listviews_returns_views_in_prefixed_relative_path()
        {
            var foundViews = new List<string>();

            _hostHomeView.ViewPath.getPathParts()
                .Union(new[] { Constants.Shared })
                .Each(path => foundViews.AddRange(_viewFolder.ListViews(path)));
                
            foundViews.ShouldHaveTheSameElementsAs(
                _hostHomeView.ViewPath,
                _hostApplicationView.ViewPath,
                _hostFooterPartialView.ViewPath);
        }

        [Test]
        public void returns_viewsource_for_partial_from_package()
        {
            readfromStream(_pak1NamePartialView.ViewPath)
                .ShouldEqual("Pak1");
        }

        [Test]
        public void returns_viewsource_for_view_from_package()
        {
            readfromStream(_pak1HomeView.ViewPath)
                .ShouldEqual(@"home from <name /><footer />");
        }

        [Test]
        public void returns_viewsource_for_master_from_package()
        {
            readfromStream(_pak2ApplicationView.ViewPath)
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
