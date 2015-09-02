using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
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
        private bool _latched;


        public FileChangeWatcher(string root, FileSet fileSet, IChangeSetHandler handler)
        {
            _files = new FubuApplicationFiles(root);
            _fileSet = fileSet;
            _handler = handler;
            _timer = new Timer
            {
                AutoReset = false
            };

            Root = root;

            _timer.Elapsed += _timer_Elapsed;
        }

        public string Root { get; private set; }

        public IEnumerable<string> CurrentFiles
        {
            get
            {
                if (_tracking == null) return Enumerable.Empty<string>();

                return _tracking.Files.Select(x => _files.RootPath.AppendPath(x));
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_latched)
            {
                processChanges();
            }

            _timer.Start();
        }

        private void processChanges()
        {
            var files = findFiles();
            var changes = _tracking.DetectChanges(files);
            if (!changes.HasChanges())
            {
                return;
            }

            if (ChangeBuffer > 0)
            {
                var checkpoint = new TrackedSet(files);
                var count = 0;
                while (count < 3)
                {
                    files = findFiles();
                    if (checkpoint.DetectChanges(files).HasChanges())
                    {
                        checkpoint = new TrackedSet(files);
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
            }


            if (!_latched)
            {
                try
                {
                    _tracking = new TrackedSet(files);
                    _handler.Handle(changes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }


        private IFubuFile[] findFiles()
        {
            return _files.FindFiles(_fileSet).ToArray();
        }


        /// <summary>
        /// Time in milliseconds that the polling watcher should wait for additional changes
        /// </summary>
        public int ChangeBuffer = 100;

        public double PollingInterval { get; set; }


        public bool Enabled { get; private set; }

        public void Latch(Action action)
        {
            _latched = true;
            try
            {
                action();
            }
            finally
            {
                _latched = false;
            }
        }

        public void Start()
        {
            _tracking = new TrackedSet(findFiles());
            _timer.Enabled = Enabled = true;
        }

        public void Stop()
        {
            Enabled = false;
            _timer.Enabled = false;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}