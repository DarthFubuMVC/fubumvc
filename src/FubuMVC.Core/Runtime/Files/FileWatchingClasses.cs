using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Web.Caching;
using FubuCore;

namespace FubuMVC.Core.Runtime.Files
{
    public class FileChangeWatcher : IDisposable
    {
        private readonly FileSet _fileSet;
        private readonly IChangeSetHandler _handler;
        private readonly Timer _timer;
        private readonly FubuApplicationFiles _files;
        private TrackedSet _tracking;
        

        public FileChangeWatcher(string root, FileSet fileSet, IChangeSetHandler handler)
        {
            _files = new FubuApplicationFiles(root);
            _fileSet = fileSet;
            _handler = handler;
            _timer = new Timer()
            {
                AutoReset = false
            };


            _timer.Elapsed += _timer_Elapsed;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Need to latch here and reset.
            // If no changes found, reset and exit
            // if changes found, fire again, merge and call through to the handler
            throw new NotImplementedException();
        }


        private IEnumerable<IFubuFile> findFiles()
        {
            return _files.FindFiles(_fileSet);
        }

        private ChangeSet findChanges()
        {
            return _tracking.DetectChanges(findFiles());
        }

        /// <summary>
        /// Time in milliseconds that the polling watcher should wait for additional changes
        /// </summary>
        public double ChangeBuffer = 100;

        public double PollingInterval { get; set; }


        public bool Enabled { get; private set; }

        public void Latch(Action action)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            _tracking = new TrackedSet(findFiles());
            Enabled = true;
        }

        public void Stop()
        {
            Enabled = false;
            _timer.Enabled = false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public interface IChangeSetHandler
    {
        void Handle(ChangeSet changes);
    }
}