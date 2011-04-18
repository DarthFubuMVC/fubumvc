using System;
using System.IO;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.Tokenization;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;
using Constants = FubuMVC.Spark.Tokenization.Constants;

namespace FubuMVC.Spark.Tests.Experiments
{
    public class SparkEngineFactoryTester : InteractionContext< SparkEngineFactory>
    {
        private readonly string _hostRoot;
        private readonly string _pak1;
        private readonly string _pak2;

        private readonly SparkItem _hostHomeView;
        private readonly SparkItem _pak1HomeView;
        private readonly SparkItem _pak2HomeView;
        private readonly SparkItem _hostApplicationView;
        private readonly SparkItem _pak2ApplicationView;
        private readonly SparkItem _pak2ThemeView;

        public SparkEngineFactoryTester()
        {
            _hostRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tokenization", "Scanning", "Templates");
            _pak1 = Path.Combine(_hostRoot, "Pak1");
            _pak2 = Path.Combine(_hostRoot, "Pak2");

            _hostHomeView = new SparkItem(Path.Combine(_hostRoot, "Home", "Home.spark"), _hostRoot, Constants.HostOrigin);
            _pak1HomeView = new SparkItem(Path.Combine(_pak1, "Home", "Home.spark"), _hostRoot, "Pak1");
            _pak2HomeView = new SparkItem(Path.Combine(_pak2, "Home", "Home.spark"), _hostRoot, "Pak2");

            _hostApplicationView = new SparkItem(Path.Combine(_hostRoot, "Shared", "application.spark"), _hostRoot, Constants.HostOrigin);
            _pak2ApplicationView = new SparkItem(Path.Combine(_pak2, "Shared", "application.spark"), _hostRoot, "Pak2");
            _pak2ThemeView = new SparkItem(Path.Combine(_pak2, "Shared", "theme.spark"), _hostRoot, "Pak2");
        }

        protected override void beforeEach()
        {
            base.beforeEach();
            ClassUnderTest.SetRoot(_hostRoot);
            ClassUnderTest.SetPackagesRoots(new[] { _pak1, _pak2 });
        }

        [Test]
        public void can_resolve_entries_from_host()
        {
            var engine = ClassUnderTest.GetEngine(_hostHomeView);
            engine.ViewFolder.HasView("Home/Home.spark").ShouldBeTrue();
        }

        [Test]
        public void can_resolve_entries_from_package()
        {
            var engine = ClassUnderTest.GetEngine(_pak1HomeView);
            engine.ViewFolder.HasView("Home/Home.spark").ShouldBeTrue();
        }

        [Test]
        public void entries_from_package_have_higher_relevance_than_from_host()
        {
            var engine = ClassUnderTest.GetEngine(_pak1HomeView);
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_pak1HomeView.RelativePath());
           
            instance = engine.CreateInstance(descriptor);
            instance.RenderView(writer);
            
            content = writer.ToString();
            content.ShouldEqual("home from Pak1");
        }

        [Test]
        public void entries_from_host_have_higher_relevance_than_from_packages()
        {
            var engine = ClassUnderTest.GetEngine(_hostHomeView);
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_hostHomeView.RelativePath());

            instance = engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("home from Root");
        }

        [Test]
        public void entries_from_packages_can_reference_entries_from_host()
        {
            var engine = ClassUnderTest.GetEngine(_pak2HomeView);
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_pak2HomeView.RelativePath()); // view
            descriptor.AddTemplate(_hostApplicationView.RelativePath()); // master, or, pak2HomeView.Master.RelativePath()

            instance = engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("<div>Host Application: home from Pak2</div>");
        }


        [Test]
        public void entries_from_packages_can_reference_entries_from_the_same_package()
        {
            var engine = ClassUnderTest.GetEngine(_pak2HomeView);
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_pak2HomeView.RelativePath()); // view
            descriptor.AddTemplate(_pak2ApplicationView.RelativePath()); // master, or, pak2HomeView.Master.RelativePath()

            instance = engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("<div>Pak2 Application: home from Pak2</div>");
        }

        [Test]
        public void entries_from_host_can_reference_entries_from_packages()
        {
            var engine = ClassUnderTest.GetEngine(_hostHomeView);
            var descriptor = new SparkViewDescriptor();
            var writer = new StringWriter();
            ISparkView instance;
            string content;

            descriptor.AddTemplate(_hostHomeView.RelativePath()); // view
            descriptor.AddTemplate(_pak2ThemeView.RelativePath()); // master, or, _hostHomeView.Master.RelativePath()

            instance = engine.CreateInstance(descriptor);
            instance.RenderView(writer);

            content = writer.ToString();
            content.ShouldEqual("<div>Pak2 Theme: home from Root</div>");
        }
    }
}