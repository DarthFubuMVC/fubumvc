using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views
{
    public class ViewIntegrationContext : IPackageLoader
    {
        public static readonly string Folder = "Views" + Guid.NewGuid();
        public static readonly IFileSystem fileSystem = new FileSystem();
        public static readonly string Application = "Application";
        private string _directory;
        private readonly IList<ContentOnlyPackageInfo> _bottles = new List<ContentOnlyPackageInfo>();
        private readonly IList<ContentStream> _streams = new List<ContentStream>();
        private InMemoryHost _host;
        private readonly string _applicationDirectory;
        protected Scenario Scenario;

        public ViewIntegrationContext()
        {
            _applicationDirectory = _directory = Folder.AppendPath(Application).ToFullPath();
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            fileSystem.DeleteDirectory(Folder);
            fileSystem.CreateDirectory(Folder);

            fileSystem.CreateDirectory(Folder.AppendPath(Application));

            _streams.Each(x => x.DumpContents());

            Thread.Sleep(100); // let the file system cool off a bit first

            FubuMvcPackageFacility.PhysicalRootPath = _applicationDirectory;

            var runtime = FubuApplication.For(determineRegistry()).StructureMap()
                .Packages(x => x.Loader(this)).Bootstrap();

            _host = new InMemoryHost(runtime);
        }

        private FubuRegistry determineRegistry()
        {
            var registryType = GetType().GetNestedTypes().FirstOrDefault(x => x.IsConcreteTypeOf<FubuRegistry>());
            return registryType == null ? new FubuRegistry() : Activator.CreateInstance(registryType).As<FubuRegistry>();
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            _host.SafeDispose();
            fileSystem.DeleteDirectory(Folder);
        }

        [SetUp]
        public void SetUp()
        {
            Scenario = _host.CreateScenario();
        }

        [TearDown]
        public void TearDown()
        {
            Scenario.As<IDisposable>().Dispose();
        }

        protected IServiceLocator Services
        {
            get
            {
                return _host.Services;
            }
        }

        protected BehaviorGraph BehaviorGraph
        {
            get
            {
                return Services.GetInstance<BehaviorGraph>();
            }
        }

        protected void InBottle(string name)
        {
            _directory = Folder.AppendPath(name).ToFullPath();
            var bottle = new ContentOnlyPackageInfo(_directory, name);
            _bottles.Add(bottle);
        }

        protected ContentStream File(string name)
        {
            var stream = new ContentStream(_directory, name, "");

            _streams.Add(stream);

            return stream;
        }

        protected ContentStream RazorView(string name)
        {
            var stream = new ContentStream(_directory, name, ".cshtml");

            _streams.Add(stream);

            return stream;
        }


        protected ContentStream RazorView<T>(string name)
        {
            var stream = new ContentStream(_directory, name, ".cshtml");
            stream.WriteLine("@model {0}", typeof(T).FullName);

            _streams.Add(stream);

            return stream;
        }

        protected ContentStream SparkView<T>(string name)
        {
            var stream = new ContentStream(_directory, name, ".spark");
            stream.WriteLine("<viewdata model=\"{0}\" />", typeof(T).FullName);

            _streams.Add(stream);

            return stream;
        }

        protected ContentStream SparkView(string name)
        {
            var stream = new ContentStream(_directory, name, ".spark");

            _streams.Add(stream);

            return stream;
        }

        IEnumerable<IPackageInfo> IPackageLoader.Load(IPackageLog log)
        {
            return _bottles;
        }

        protected ViewBag Views
        {
            get
            {
                return _host.Services.GetInstance<ViewBag>();
            }
        }

        protected RazorViewFacility RazorViews
        {
            get
            {
                return _host.Services.GetInstance<ViewEngineSettings>().Facilities.OfType<RazorViewFacility>().Single();
            }
        }

        protected ITemplateFile ViewForModel<T>()
        {
            return Views.ViewsFor(typeof (T)).OfType<ITemplateFile>().FirstOrDefault();
        }
    }

    public class ContentStream
    {
        private readonly string _path;
        private readonly StringWriter _writer = new StringWriter();

        public ContentStream(string folder, string name, string extension)
        {
            _path = Path.Combine(folder, name + extension);
        }

        public ContentStream WriteLine(string format, params object[] parameters)
        {
            _writer.WriteLine(format, parameters);
            return this;
        }

        public void Write(string text)
        {
            _writer.WriteLine(text.Replace("'", "\"").TrimStart());
        }

        public void DumpContents()
        {
            ViewIntegrationContext.fileSystem.WriteStringToFile(_path, _writer.ToString());
        }
    }

    public class ViewModel1{}
    public class ViewModel2{}
    public class ViewModel3{}
    public class ViewModel4{}
}