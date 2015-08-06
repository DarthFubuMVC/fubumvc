using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.IntegrationTesting.Views;
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
        private FubuRuntime _host;
        private readonly string _applicationDirectory;
        protected Scenario Scenario;
        private Lazy<AssetGraph> _allAssets;

        public string Mode = null;

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

            var registry = determineRegistry();
            registry.Mode = Mode;
            
            registry.RootPath = _applicationDirectory;

            var runtime = registry.ToRuntime();

            _host = runtime;

            _allAssets = new Lazy<AssetGraph>(() => { return runtime.Get<IAssetFinder>().FindAll(); });
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
            Scenario = new Scenario(_host);
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
            return _host.Get<IAssetTagBuilder>();
        }


        public IAssetFinder Assets
        {
            get { return _host.Get<IAssetFinder>(); }
        }
    }
}