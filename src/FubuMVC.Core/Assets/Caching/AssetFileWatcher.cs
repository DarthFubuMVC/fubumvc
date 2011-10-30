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
        private readonly IAssetPipeline _pipeline;
        private readonly IAssetFileChangeListener _listener;
        private readonly AssetFileMonitoringSettings _settings;
        private FileChangePollingWatcher _watcher;
            

        public AssetFileWatcher(IAssetPipeline pipeline, IAssetFileChangeListener listener, AssetFileMonitoringSettings settings)
        {
            _pipeline = pipeline;
            _listener = listener;
            _settings = settings;
        }

        public void StartWatchingAll()
        {
            _watcher = new FileChangePollingWatcher();
            _pipeline.AllFiles().Each(file =>
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