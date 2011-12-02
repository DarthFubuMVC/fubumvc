using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core;

namespace Fubu.Applications
{
    public class FubuMvcApplicationFileWatcher : IDisposable
    {
        private readonly IApplicationDomain _applicationDomain;
        private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        public FubuMvcApplicationFileWatcher(IApplicationDomain applicationDomain)
        {
            _applicationDomain = applicationDomain;
        }

        public void StartWatching(ApplicationSettings settings, IEnumerable<string> folders)
        {
            cleanUpWatchers();

            Console.WriteLine("Listening for file changes at");

            addDirectory(settings.PhysicalPath);
            folders.Each(addDirectory);
        }

        private void addDirectory(string directory)
        {
            var watcher = new FileSystemWatcher(directory){
                IncludeSubdirectories = true
            };

            watcher.Created += fileChanged;
            watcher.Deleted += fileChanged;
            watcher.Changed += fileChanged;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine(" - " + directory);

            _watchers.Add(watcher);
        }



        void fileChanged(object sender, FileSystemEventArgs e)
        {
            ChangeFile(e.Name);
        }

        public void ChangeFile(string filename)
        {
            switch (determineCategory(filename))
            {
                case FileChangeEventCategory.NothingImportant:
                    return;

                case FileChangeEventCategory.ContentShouldRecycle:
                    _applicationDomain.RecycleContent();
                    break;

                case FileChangeEventCategory.DomainShouldRecycle:
                    _applicationDomain.RecycleDomain();
                    break;
            }
        }

        private static FileChangeEventCategory determineCategory(string filename)
        {
            var extension = Path.GetExtension(filename).ToLower();

            if (extension == ".dll" || extension == ".exe") return FileChangeEventCategory.DomainShouldRecycle;


            var parts = filename.ToLower().Split(Path.DirectorySeparatorChar);
            if (parts.Last() == "web.config")
            {
                return FileChangeEventCategory.DomainShouldRecycle;
            }

            if (parts.Contains("bin")) return FileChangeEventCategory.DomainShouldRecycle;

            if (extension == ".config") return FileChangeEventCategory.ContentShouldRecycle;

            return FileChangeEventCategory.NothingImportant;
        }

        // tear down and reconstruct if it's a dll or exe.  
        // recycle if it's a *.config file
        public void Dispose()
        {
            cleanUpWatchers();
        }

        private void cleanUpWatchers()
        {
            _watchers.Each(x =>
            {
                x.EnableRaisingEvents = false;
                x.SafeDispose();
            });
        }
    }
}