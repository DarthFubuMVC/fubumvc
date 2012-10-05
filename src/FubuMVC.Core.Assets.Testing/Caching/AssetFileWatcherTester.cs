using System;
using System.Collections.Generic;
using System.Threading;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Caching
{
    [TestFixture]
    public class AssetFileWatcherTester 
    {

        [Test]
        public void asset_file_watcher_should_be_registered_as_a_singleton()
        {
            BehaviorGraph.BuildFrom(x => x.Import<AssetBottleRegistration>())
                .Services
                .DefaultServiceFor<IAssetFileWatcher>()
                .Type.ShouldEqual(typeof(AssetFileWatcher));

            ServiceRegistry.ShouldBeSingleton(typeof(AssetFileWatcher)).ShouldBeTrue();
        }

        private AssetFile fileFor(string fileName)
        {
            new FileSystem().WriteStringToFile(fileName, "start");

            return new AssetFile(fileName){
                FullPath = fileName
            };
        }

        private void alter(AssetFile file)
        {
            new FileSystem().AlterFlatFile(file.FullPath, list => list.Add("something"));
        }

        [Test]
        public void big_integrated_smoke_test()
        {
            var pipeline = new StubAssetFileGraph();
            var listener = MockRepository.GenerateMock<IAssetFileChangeListener>();


            var watcher = new AssetFileWatcher(pipeline, listener, new AssetFileMonitoringSettings{
                MonitoringIntervalTime = 100
            });

            try
            {
                var file1 = pipeline.AddFile("1");
                var file2 = pipeline.AddFile("2");
                var file3 = pipeline.AddFile("3");
                var file4 = pipeline.AddFile("4");
                var file5 = pipeline.AddFile("5");
				
				Thread.Sleep(1001);
				
                watcher.StartWatchingAll();

                listener.AssertWasNotCalled(x => x.Changed(null), x => x.IgnoreArguments());

                pipeline.AlterFile("1");
                pipeline.AlterFile("3");
                pipeline.AlterFile("5");

                Thread.Sleep(1001);

                listener.AssertWasCalled(x => x.Changed(file1));
                listener.AssertWasNotCalled(x => x.Changed(file2));
                listener.AssertWasCalled(x => x.Changed(file3));
                listener.AssertWasNotCalled(x => x.Changed(file4));
                listener.AssertWasCalled(x => x.Changed(file5));
            }
            finally
            {
                watcher.StopWatching();
            }
        }        
    }

    public class StubAssetFileGraph : IAssetFileGraph
    {
        private readonly Cache<string, AssetFile> _files = new Cache<string, AssetFile>();
        private readonly IFileSystem _system = new FileSystem();

        public AssetFile AddFile(string name)
        {
            var file = new AssetFile(name){
                FullPath = name + ".txt"
            };

            _system.WriteStringToFile(file.FullPath, Guid.NewGuid().ToString());

            _files[name] = file;

            return file;
        }

        public void AlterFile(string name)
        {
            _system.AlterFlatFile(_files[name].FullPath, list => list.Add(Guid.NewGuid().ToString()));
        }

        public AssetFile Find(string path)
        {
            throw new NotImplementedException();
        }

        public AssetPath AssetPathOf(AssetFile file)
        {
            throw new NotImplementedException();
        }

        public AssetFile FindByPath(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetFile> AllFiles()
        {
            return _files;
        }

        public AssetFile Find(AssetPath path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PackageAssets> AllPackages
        {
            get { throw new NotImplementedException(); }
        }
    }
}