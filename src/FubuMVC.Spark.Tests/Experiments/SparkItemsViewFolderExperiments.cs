using System;
using System.IO;
using System.Linq;
using FubuMVC.Spark.Tokenization;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;
using Spark.Parser;
using Constants = FubuMVC.Spark.Tokenization.Constants;
using Spark.FileSystem;
using System.Collections.Generic;

namespace FubuMVC.Spark.Tests.Experiments
{
    public class SparkItemsViewFolderExperiments
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


        private readonly ISparkViewEngine _engine;

        public SparkItemsViewFolderExperiments()
        {
            _hostRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            _pak1 = Path.Combine(_hostRoot, "Pak1");
            _pak2 = Path.Combine(_hostRoot, "Pak2");

            _hostHomeView = new SparkItem(Path.Combine(_hostRoot, "Home", "Home.spark"), _hostRoot, Constants.HostOrigin);
            _hostApplicationView = new SparkItem(Path.Combine(_hostRoot, "Shared", "application.spark"), _hostRoot, Constants.HostOrigin);
           
            _pak1HomeView = new SparkItem(Path.Combine(_pak1, "Home", "Home.spark"), _hostRoot, "P1");
            _pak1NamePartialView = new SparkItem(Path.Combine(_pak1, "Home", "_name.spark"), _hostRoot, "P1");
            
            _pak2HomeView = new SparkItem(Path.Combine(_pak2, "Home", "Home.spark"), _hostRoot, "P2");
            _pak2ApplicationView = new SparkItem(Path.Combine(_pak2, "Shared", "application.spark"), _hostRoot, "P2");
            _pak2ThemeView = new SparkItem(Path.Combine(_pak2, "Shared", "theme.spark"), _hostRoot, "P2");

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

            var settings = new SparkSettings();
            _engine = new SparkViewEngine(settings);
            _engine.ViewFolder = _engine.ViewFolder.Append(new SparkItemViewFolder(sparkItems.Where(x => x.Origin == "P1").ToList()));
            _engine.ViewFolder = _engine.ViewFolder.Append(new SparkItemViewFolder(sparkItems.Where(x => x.Origin == "P2").ToList()));
            _engine.ViewFolder = _engine.ViewFolder.Append(new SparkItemViewFolder(sparkItems.Where(x => x.Origin == Constants.HostOrigin).ToList()));
        }

        [Test]
        public void list_views_returns_correct_paths()
        {
            new ViewLoader {ViewFolder = new SparkItemViewFolder(new []{_pak1HomeView, _pak1NamePartialView})}
                .FindPartialFiles(_pak1HomeView.RelativePath())
                .ShouldContain("name");
        }

        [Test]
        public void can_resolve_entries_from_host()
        {
            _engine.ViewFolder.HasView(_hostHomeView.RelativePath()).ShouldBeTrue();
        }

        [Test]
        public void can_resolve_entries_from_package()
        {
            _engine.ViewFolder.HasView(_pak1HomeView.RelativePath()).ShouldBeTrue();
        }

        [Test]
        public void entries_from_package_have_higher_relevance_than_from_host()
        {
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_pak1HomeView.RelativePath());
           
            instance = _engine.CreateInstance(descriptor);
            instance.RenderView(writer);
            
            content = writer.ToString();
            content.ShouldEqual("home from Pak1");
        }

        [Test]
        public void entries_from_host_have_higher_relevance_than_from_packages()
        {
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_hostHomeView.RelativePath());

            instance = _engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("home from Root");
        }

        [Test]
        public void entries_from_packages_can_reference_entries_from_host()
        {
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_pak2HomeView.RelativePath()); // view
            descriptor.AddTemplate(_hostApplicationView.RelativePath()); // master, or, pak2HomeView.Master.RelativePath()

            instance = _engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("<div>Host Application: home from Pak2</div>");
        }


        [Test]
        public void entries_from_packages_can_reference_entries_from_the_same_package()
        {
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_pak2HomeView.RelativePath()); // view
            descriptor.AddTemplate(_pak2ApplicationView.RelativePath()); // master, or, pak2HomeView.Master.RelativePath()

            instance = _engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("<div>Pak2 Application: home from Pak2</div>");
        }

        [Test]
        public void entries_from_host_can_reference_entries_from_packages()
        {
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_hostHomeView.RelativePath()); // view
            descriptor.AddTemplate(_pak2ThemeView.RelativePath()); // master, or, _hostHomeView.Master.RelativePath()

            instance = _engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("<div>Pak2 Theme: home from Root</div>");
        }
    }
}