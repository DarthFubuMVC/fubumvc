using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu.Running
{
    public class FubuMvcApplicationFileWatcher : IDisposable
    {
        private readonly IFileMatcher _matcher;
        private readonly IApplicationObserver _observer;
        private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        public FubuMvcApplicationFileWatcher(IApplicationObserver observer, IFileMatcher matcher)
        {
            _observer = observer;
            _matcher = matcher;
        }

        public bool Latched { get; set; }

        public void Dispose()
        {
            cleanUpWatchers();
        }

        public void StartWatching(string physicalPath, IEnumerable<string> folders)
        {
            cleanUpWatchers();

            Console.WriteLine("Listening for file changes at");

            addDirectory(physicalPath);
            folders.Each(addDirectory);
        }

        public void StopWatching()
        {
            cleanUpWatchers();
        }

        private void addDirectory(string directory)
        {
            // This gets rid of issues from having non-existent Data folders
            if (!Directory.Exists(directory)) return;

            var watcher = new FileSystemWatcher(directory)
            {
                IncludeSubdirectories = true
            };

            watcher.Created += fileChanged;
            watcher.Deleted += fileChanged;
            watcher.Changed += fileChanged;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine(" - " + directory);

            _watchers.Add(watcher);
        }


        private void fileChanged(object sender, FileSystemEventArgs e)
        {
            ChangeFile(e.Name);
        }

        public void ChangeFile(string filename)
        {
            if (Latched) return;

            var category = _matcher.CategoryFor(filename);

            ConsoleWriter.Write(ConsoleColor.Gray, "Detected change to file " + filename);

            switch (category)
            {
                case FileChangeCategory.AppDomain:
                    _observer.RecycleAppDomain();
                    break;

                case FileChangeCategory.Application:
                    _observer.RecycleApplication();
                    break;

                case FileChangeCategory.Content:
                    _observer.RefreshContent();
                    break;
            }
        }


        // tear down and reconstruct if it's a dll or exe.  
        // recycle if it's a *.config file

        private void cleanUpWatchers()
        {
            _watchers.Each(x => {
                x.EnableRaisingEvents = false;
                x.SafeDispose();
            });

            _watchers.Clear();
        }
    }
}