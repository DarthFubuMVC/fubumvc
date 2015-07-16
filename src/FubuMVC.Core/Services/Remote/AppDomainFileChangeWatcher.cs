using System;
using System.Collections.Generic;
using System.IO;

namespace FubuMVC.Core.Services.Remote
{
    public class AppDomainFileChangeWatcher : IDisposable
    {
        private readonly Action _callback;
        private DateTime _lastUpdate;
        private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>(); 

        public AppDomainFileChangeWatcher(Action callback)
        {
            _callback = callback;
        }

        public void WatchBinariesAt(string directory)
        {
            cleanUpWatcher();

            addWatcher(directory, "*.dll");
            addWatcher(directory, "*.config");

            _lastUpdate = DateTime.Now;
        }

        private void addWatcher(string directory, string pattern)
        {
            var watcher = new FileSystemWatcher(directory, pattern);
            watcher.Changed += fileChanged;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            

            _watchers.Add(watcher);
        }

        private void fileChanged(object sender, FileSystemEventArgs e)
        {
            if (DateTime.Now.Subtract(_lastUpdate).TotalSeconds < 3)
            {
                return;
            }

            Console.WriteLine("Detected change to file at " + e.FullPath);
            _callback();
            _lastUpdate = DateTime.Now;
        }

        private void cleanUpWatcher()
        {
            _watchers.Each(watcher => {
                try
                {
                    watcher.Changed -= fileChanged;
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
                catch (Exception)
                {
                }
            });

            _watchers.Clear();
        }

        public void Dispose()
        {
            cleanUpWatcher();
        }
    }
}