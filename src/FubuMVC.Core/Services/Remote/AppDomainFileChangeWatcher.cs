using System;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.Services.Remote
{
    public class AppDomainFileChangeWatcher : IDisposable, IChangeSetHandler
    {
        private readonly Action _callback;
        private FileChangeWatcher _watcher;

        public AppDomainFileChangeWatcher(Action callback)
        {
            _callback = callback;
        }

        public void WatchBinariesAt(string directory)
        {
            Console.WriteLine("Watching for binary and config file changes at " + directory);

            _watcher = new FileChangeWatcher(directory, FileSet.Deep("*.dll;*.config;*.exe"), this)
            {
                ChangeBuffer = 500
            };



            _watcher.Start();
        }


        public void Dispose()
        {
            _watcher.Dispose();
        }

        void IChangeSetHandler.Handle(ChangeSet changes)
        {
            Console.WriteLine("Detected binary file changes at {0}: {1}", _watcher.Root, changes);
            _callback();
        }

        public void StopWatching()
        {
            _watcher.Stop();
        }

        public void StartWatching()
        {
            _watcher.Start();
        }
    }
}