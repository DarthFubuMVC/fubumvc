using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Bottles;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.OwinHost;

namespace Serenity.Jasmine
{
    public class JasmineRunner : ISpecFileListener
    {
        private readonly InteractiveJasmineInput _input;
        private readonly Thread _kayakLoop;
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private SerenityJasmineApplication _application;
        private ApplicationUnderTest _applicationUnderTest;
        private FubuOwinHost _host;
        private AssetFileWatcher _watcher;


        public JasmineRunner(InteractiveJasmineInput input)
        {
            _input = input;
            var threadStart = new ThreadStart(run);
            _kayakLoop = new Thread(threadStart);
        }

        // TODO -- this will get changed to Run(file), or we'll bring file thru a ctor
        public void Run()
        {
            buildApplication();
            _kayakLoop.Start();


            // TODO -- make a helper method for this
            _applicationUnderTest.Driver.Navigate().GoToUrl(_applicationUnderTest.RootUrl);

            _reset.WaitOne();
        }

        private void run()
        {
            _host = new FubuOwinHost(_application);
            _host.RunApplication(_input.PortFlag, watchAssetFiles);

            _reset.Set();
        }

        private void watchAssetFiles(FubuRuntime runtime)
        {
            if (_watcher == null)
            {
                _watcher = runtime.Facility.Get<AssetFileWatcher>();
                _watcher.StartWatching(this); 
            }


        }

        void ISpecFileListener.Changed()
        {
            _applicationUnderTest.Driver.Navigate().Refresh();
        }

        void ISpecFileListener.Deleted()
        {
            _host.Recycle(watchAssetFiles);
            // TODO -- make a helper method for this
            _applicationUnderTest.Driver.Navigate().GoToUrl(_applicationUnderTest.RootUrl);
        }

        void ISpecFileListener.Added()
        {
            _host.Recycle(watchAssetFiles);
            _applicationUnderTest.Driver.Navigate().Refresh();
        }


        private void buildApplication()
        {
            _application = new SerenityJasmineApplication();
            var configLoader = new ConfigFileLoader(_input.SerenityFile, _application);
            configLoader.ReadFile();


            var applicationSettings = new ApplicationSettings{
                RootUrl = "http://localhost:" + _input.PortFlag
            };

            var browserBuilder = _input.GetBrowserBuilder();

            _applicationUnderTest = new ApplicationUnderTest(_application, applicationSettings, browserBuilder);
        }
    }

    public class AssetFileWatcher
    {
        private readonly IAssetContentCache _cache;
        private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private readonly IFileSystem _fileSystem = new FileSystem();

        public AssetFileWatcher(IAssetContentCache cache)
        {
            _cache = cache;
        }

        public void StartWatching(ISpecFileListener listener)
        {

            PackageRegistry.Packages.Each(pak =>
            {
                pak.ForFolder(BottleFiles.WebContentFolder, dir =>
                {
                    var contentFolder = dir.AppendPath("content");
                    if (_fileSystem.DirectoryExists(contentFolder))
                    {
                        addContentFolder(contentFolder, listener);
                    }
                });
            });
        }

        private void addContentFolder(string dir, ISpecFileListener listener)
        {
            var watcher = new FileSystemWatcher(dir);
            watcher.Changed += (x, y) =>
            {
                Console.WriteLine("Detected a change to " + y.FullPath);
                _cache.FlushAll();
                listener.Changed();
            };

            watcher.Created += (x, y) =>
            {
                Console.WriteLine("Detected a new file at " + y.FullPath);
                listener.Added();
            };
            watcher.Deleted += (x, y) =>
            {
                Console.WriteLine("Detected a file deletion at " + y.FullPath);
                listener.Deleted();
            };

            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
        }

        public void StopWatching()
        {
            _watchers.Each(x => x.SafeDispose());
        }
    }

    public interface ISpecFileListener
    {
        void Changed();
        void Deleted();
        void Added();
    }
}