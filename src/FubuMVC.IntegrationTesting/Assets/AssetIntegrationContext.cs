using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http.Scenarios;
using Xunit;

namespace FubuMVC.IntegrationTesting.Assets
{
    public class AssetIntegrationContext : IDisposable
    {
        public static readonly string Folder = "Assets" + Guid.NewGuid();
        public static readonly IFileSystem fileSystem = new FileSystem();
        public static readonly string Application = "Application";
        private readonly string _applicationDirectory;
        private readonly string _directory;
        private readonly IList<ContentStream> _streams = new List<ContentStream>();
        private Lazy<AssetGraph> _allAssets;
        private FubuRuntime _host;

        public string Mode = null;
        protected Scenario Scenario;

        public AssetGraph AllAssets
        {
            get { return _allAssets.Value; }
        }


        public IAssetFinder Assets
        {
            get { return _host.Get<IAssetFinder>(); }
        }

        public AssetIntegrationContext()
        {
            _applicationDirectory = _directory = Folder.AppendPath(Application).ToFullPath();

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

            Scenario = new Scenario(_host);
        }



        private FubuRegistry determineRegistry()
        {
            var registryType = GetType().GetNestedTypes().FirstOrDefault(x => x.IsConcreteTypeOf<FubuRegistry>());
            return registryType == null ? new FubuRegistry() : Activator.CreateInstance(registryType).As<FubuRegistry>();
        }

        public void Dispose()
        {
            _host.SafeDispose();
            fileSystem.DeleteDirectory(Folder);

            Scenario.As<IDisposable>().Dispose();
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
    }
}