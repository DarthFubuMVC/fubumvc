using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.IntegrationTesting.Views;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Assets
{
    public class AssetIntegrationContext
    {
        public static readonly string Folder = "Assets" + Guid.NewGuid();
        public static readonly IFileSystem fileSystem = new FileSystem();
        public static readonly string Application = "Application";
        private readonly string _directory;
        private readonly IList<ContentStream> _streams = new List<ContentStream>();
        private InMemoryHost _host;
        private readonly string _applicationDirectory;
        protected Scenario Scenario;
        private Lazy<AssetGraph> _allAssets;

        public AssetIntegrationContext()
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

            FubuApplication.PhysicalRootPath = _applicationDirectory;

            var runtime = FubuApplication.For(determineRegistry()).StructureMap().Bootstrap();

            _host = new InMemoryHost(runtime);

            _allAssets = new Lazy<AssetGraph>(() => { return runtime.Factory.Get<IAssetFinder>().FindAll(); });
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

        public AssetGraph AllAssets
        {
            get { return _allAssets.Value; }
        }

        protected ContentStream File(string name)
        {
            var stream = new ContentStream(_directory, name, "");

            _streams.Add(stream);

            return stream;
        }

        public IAssetTagBuilder TagBuilder()
        {
            return _host.Services.GetInstance<IAssetTagBuilder>();
        }


        public IAssetFinder Assets
        {
            get { return _host.Services.GetInstance<IAssetFinder>(); }
        }
    }
}