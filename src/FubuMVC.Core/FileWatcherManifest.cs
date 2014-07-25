using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Bottles;
using Bottles.Diagnostics;
using Bottles.PackageLoaders.LinkedFolders;
using FubuCore;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    public class FileWatcherManifest : IDisposable
    {
        public string ConfigurationFile = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        public string[] ContentMatches = new string[0];

        public string PublicAssetFolder = string.Empty;
        
        public string[] AssetExtensions = new string[0];

        public string ApplicationPath = FubuMvcPackageFacility.GetApplicationPath();
        public string BinPath = FubuMvcPackageFacility.FindBinPath();
                
        public string[] LinkedFolders = determineLinkedFolders();

        private static string[] determineLinkedFolders()
        {
            var loader = new LinkedFolderPackageLoader(FubuMvcPackageFacility.GetApplicationPath(), x => x);
            var packages = loader.Load(new PackageLog());

            var links = new List<string>();

            packages.Each(x => x.ForFolder(BottleFiles.WebContentFolder, links.Add));

            return links.ToArray();
        }

        private readonly IList<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();


        public void Watch(bool refreshContent, IApplicationObserver observer)
        {
            FileSystemEventHandler appDomain = (o, args) => {
                StopWatching();
                observer.RecycleAppDomain();
            };

            FileSystemEventHandler reload = (o, args) => {
                var watcher = o.As<FileSystemWatcher>();

                watcher.EnableRaisingEvents = false;

                try
                {
                    observer.RefreshContent();
                }
                finally
                {
                    watcher.EnableRaisingEvents = true;
                }
            };

            add(BinPath, "*.dll").OnChangeOrCreation(appDomain);
            add(BinPath, "*.exe").OnChangeOrCreation(appDomain);

            if (ConfigurationFile.IsNotEmpty())
            {
                add(ApplicationPath, ConfigurationFile).OnChangeOrCreation(appDomain);
            }

            if (!refreshContent)
            {
                return;
            }

            if (PublicAssetFolder.IsNotEmpty())
            {
                add(PublicAssetFolder, "*.js").OnChange(reload);
                add(PublicAssetFolder, "*.jsx").OnChange(reload);
                add(PublicAssetFolder, "*.css").OnChange(reload);
            }
            else
            {
                add(ApplicationPath, "*.js").OnChange(reload);
                add(ApplicationPath, "*.jsx").OnChange(reload);
                add(ApplicationPath, "*.css").OnChange(reload);

                LinkedFolders.Each(x => {
                    add(x, "*.js").OnChange(reload);
                    add(x, "*.jsx").OnChange(reload);
                    add(x, "*.css").OnChange(reload);
                });
            }

            ContentMatches.Each(ext => {
                if (!ext.StartsWith("*")) ext = "*" + ext;

                add(ApplicationPath, ext).OnChange(reload);

                LinkedFolders.Each(folder => add(folder, ext).OnChangeOrCreation(reload));
            });
        }

        private FileSystemWatcher add(string directory, string pattern)
        {
            Console.WriteLine("Watching {0} for changes to {1}", directory, pattern);

            var watcher = new FileSystemWatcher(directory, pattern)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                IncludeSubdirectories = true
            };

            _watchers.Add(watcher);

            return watcher;
        }

        public bool IsWatching()
        {
            return _watchers.Any(x => x.EnableRaisingEvents);
        }

        public void StopWatching()
        {
            _watchers.Each(x => x.EnableRaisingEvents = false);
        }

        public void StartWatching()
        {
            _watchers.Each(x => x.EnableRaisingEvents = true);
        }

        public void Dispose()
        {
            StopWatching();
            _watchers.Each(x => x.SafeDispose());
            _watchers.Clear();
        }

    }

    public static class FileSystemWatcherExtensions
    {
        public static FileSystemWatcher OnCreation(this FileSystemWatcher watcher, FileSystemEventHandler handler)
        {
            watcher.Created += handler;

            return watcher;
        }

        public static FileSystemWatcher OnChange(this FileSystemWatcher watcher, FileSystemEventHandler handler)
        {
            watcher.Changed += handler;

            return watcher;
        }

        public static FileSystemWatcher OnChangeOrCreation(this FileSystemWatcher watcher, FileSystemEventHandler handler)
        {
            watcher.Changed += handler;
            watcher.Created += handler;

            return watcher;
        }
    }
}