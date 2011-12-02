using System;
using System.Collections.Generic;
using System.IO;
using Bottles;
using FubuKayak;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuCore;
using System.Linq;

namespace Fubu.Applications
{
    public interface IApplicationDomain
    {
        RecycleResponse RecycleContent();
        RecycleResponse RecycleDomain();
    }

    public class KayakApplicationDomain : IApplicationDomain
    {
        public ApplicationStartResponse Start(ApplicationSettings settings)
        {
            throw new NotImplementedException();
        }

        public RecycleResponse RecycleContent()
        {
            throw new NotImplementedException();
        }

        public RecycleResponse RecycleDomain()
        {
            throw new NotImplementedException();
        }
    }

    public enum FileChangeEventCategory
    {
        NothingImportant,
        DomainShouldRecycle,
        ContentShouldRecycle
    }

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


    public enum ApplicationStartStatus
    {
        Started,
        ApplicationSourceFailure,
        CouldNotResolveApplicationSource
    }

    [Serializable]
    public class ApplicationStartResponse
    {
        public ApplicationStartStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public string[] ApplicationSourceTypes { get; set; }
        public string[] BottleDirectories { get; set; }
    }

    [Serializable]
    public class RecycleResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }

    public class ApplicationRunner : MarshalByRefObject
    {
        private readonly IApplicationSourceFinder _sourceFinder;
        private FubuKayakApplication _kayakApplication;

        public ApplicationRunner() : this(new ApplicationSourceFinder(new ApplicationSourceTypeFinder()))
        {
        }

        public ApplicationRunner(IApplicationSourceFinder sourceFinder)
        {
            _sourceFinder = sourceFinder;
        }

        public ApplicationStartResponse StartApplication(ApplicationSettings settings)
        {
            var response = new ApplicationStartResponse();

            try
            {
                var source = _sourceFinder.FindSource(settings, response);
                if (source == null)
                {
                    response.Status = ApplicationStartStatus.CouldNotResolveApplicationSource;
                }
                else
                {
                    StartApplication(source, settings);
                    var list = new List<string>();
                    PackageRegistry.Packages.Each(pak =>
                    {
                        pak.ForFolder(BottleFiles.WebContentFolder, list.Add);
                        pak.ForFolder(BottleFiles.BinaryFolder, list.Add);
                        pak.ForFolder(BottleFiles.DataFolder, list.Add);
                    });

                    response.BottleDirectories = list.ToArray();
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.ToString();
                response.Status = ApplicationStartStatus.ApplicationSourceFailure;
            }

            return response;
        }

        public RecycleResponse Recycle()
        {
            try
            {
                _kayakApplication.Recycle(r => { });
                return new RecycleResponse{
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new RecycleResponse{
                    Success = false,
                    Message = e.ToString()
                };
            }
        }

        public virtual void StartApplication(IApplicationSource source, ApplicationSettings settings)
        {
            FubuMvcPackageFacility.PhysicalRootPath = settings.GetApplicationFolder();
            _kayakApplication = new FubuKayakApplication(source);

            // Need to make this capture the package registry failures cleanly
            _kayakApplication.RunApplication(settings.Port, r => { });
        }


    }

    public interface IApplicationSourceFinder
    {
        IApplicationSource FindSource(ApplicationSettings settings, ApplicationStartResponse theResponse);
    }

    public class ApplicationSourceFinder : IApplicationSourceFinder
    {
        private readonly IApplicationSourceTypeFinder _typeFinder;

        public ApplicationSourceFinder(IApplicationSourceTypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public IApplicationSource FindSource(ApplicationSettings settings, ApplicationStartResponse theResponse)
        {
            Type type = findType(settings, theResponse);
            return type == null ? null : (IApplicationSource)Activator.CreateInstance(type);
        }

        private Type findType(ApplicationSettings settings, ApplicationStartResponse theResponse)
        {
            if (settings.ApplicationSourceName.IsNotEmpty())
            {
                return Type.GetType(settings.ApplicationSourceName);
            }

            var types = _typeFinder.FindApplicationSourceTypes();
            theResponse.ApplicationSourceTypes = types.Select(x => x.AssemblyQualifiedName).ToArray();

            if (!types.Any()) return null;

            return only(types) ?? matchingTypeName(settings, types);
        }

        private static Type only(IEnumerable<Type> types)
        {
            return types.Count() == 1 ? types.Single() : null;
        }

        private static Type matchingTypeName(ApplicationSettings settings, IEnumerable<Type> types)
        {
            return types.FirstOrDefault(x => x.Name == settings.Name);
        }
    }

    public interface IApplicationSourceTypeFinder
    {
        IEnumerable<Type> FindApplicationSourceTypes();
    }
}