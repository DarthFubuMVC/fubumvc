using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Assets.Caching
{
    [Singleton]
    public class AssetFileWatcher : IAssetFileWatcher, IDisposable
    {
        private readonly IAssetFileGraph _fileGraph;
        private readonly IAssetFileChangeListener _listener;
        private readonly AssetFileMonitoringSettings _settings;
        private FileChangePollingWatcher _watcher;
            

        public AssetFileWatcher(IAssetFileGraph fileGraph, IAssetFileChangeListener listener, AssetFileMonitoringSettings settings)
        {
            _fileGraph = fileGraph;
            _listener = listener;
            _settings = settings;
        }

        public void StartWatchingAll()
        {
            _watcher = new FileChangePollingWatcher();
            _fileGraph.AllFiles().Each(file =>
            {
                _watcher.WatchFile(file.FullPath, () => _listener.Changed(file));
            });

            _watcher.StartWatching(_settings.MonitoringIntervalTime);
        }

        public void StopWatching()
        {
            _watcher.Stop();
            _watcher = null;
        }

        public void Dispose()
        {
            StopWatching();
        }
    }
}