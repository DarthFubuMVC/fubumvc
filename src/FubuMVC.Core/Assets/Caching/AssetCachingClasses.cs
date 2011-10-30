using System;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration;
using System.Collections.Generic;

namespace FubuMVC.Core.Assets.Caching
{

    public interface IAssetFileChangeListener
    {
        void Changed(AssetFile file);
    }


    // TODO -- make this editable somewhere
    public class AssetFileMonitoringSettings
    {
        public AssetFileMonitoringSettings()
        {
            MonitoringIntervalTime = 5000;
        }

        public double MonitoringIntervalTime { get; set; }
    }


    public class AssetFileWatchingActivator : IActivator
    {
        private readonly IAssetFileWatcher _watcher;

        public AssetFileWatchingActivator(IAssetFileWatcher watcher)
        {
            _watcher = watcher;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _watcher.StartWatchingAll();
        }
    }

    public interface IAssetFileWatcher
    {
        void StartWatchingAll();
        void StopWatching();
    }

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