using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Services.Remote;

namespace FubuMVC.Core.Assets
{
    public class FileWatcherManifest : IDisposable
    {
        public string ConfigurationFile = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        public string[] ContentMatches = new string[0];

        public string PublicAssetFolder = string.Empty;

        public string[] AssetExtensions = new string[0];

        public readonly string ApplicationPath;
        public readonly string BinPath;

        private AppDomainFileChangeWatcher _appDomainWatcher;
        private FileChangeWatcher _watcher;

        public FileWatcherManifest(string applicationPath, string binPath)
        {
            ApplicationPath = applicationPath;
            BinPath = binPath;
        }

        public void Watch(bool refreshContent, IApplicationObserver observer)
        {
            _appDomainWatcher = new AppDomainFileChangeWatcher(observer.RecycleAppDomain);
            _appDomainWatcher.WatchBinariesAt(BinPath);


            if (!refreshContent)
            {
                return;
            }

            var watchedDirectory = PublicAssetFolder.IsNotEmpty() ? PublicAssetFolder : ApplicationPath;
            var pattern = "*.css;*.js";
            ContentMatches.Each(ext =>
            {
                if (!ext.StartsWith("*")) ext = "*" + ext;
                pattern += ";" + ext;
            });

            var assetFileSet = FileSet.Deep(pattern);

            _watcher = new FileChangeWatcher(watchedDirectory, assetFileSet, new ContentRefresher(observer));
        }

        public class ContentRefresher : IChangeSetHandler
        {
            private readonly IApplicationObserver _observer;

            public ContentRefresher(IApplicationObserver observer)
            {
                _observer = observer;
            }

            public void Handle(ChangeSet changes)
            {
                _observer.RefreshContent();
            }
        }


        public bool IsWatching()
        {
            return _watcher.Enabled;
        }

        public void StopWatching()
        {
            _appDomainWatcher.StopWatching();
            _watcher.Stop();
        }

        public void StartWatching()
        {
            _appDomainWatcher.StartWatching();
            _watcher.Start();
        }

        public void Dispose()
        {
            StopWatching();
            _watcher.Dispose();
        }
    }

}