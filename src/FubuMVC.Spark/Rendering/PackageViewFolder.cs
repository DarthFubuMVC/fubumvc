//NOTE: WORK IN PROGRESS

//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Bottles;
//using FubuCore;
//using Spark.FileSystem;

//namespace FubuMVC.Spark.Rendering
//{
//    public class PackageViewFolder : IViewFolder
//    {
//        private IEnumerable<IPackageInfo> _packages = PackageRegistry.Packages;
//        private IViewFinder _viewFinder = new DefaultViewFinder();
//        private readonly string _packageName;
//        private readonly string _virtualFolderRoot;
//        private readonly string _prefix;

//        public PackageViewFolder(string packageName, string virtualFolderRoot)
//        {
//            _packageName = packageName;
//            _virtualFolderRoot = virtualFolderRoot;
//            _prefix = _packageName + Path.DirectorySeparatorChar;
//        }

//        public IPackageInfo Package { get { return _packages.First(x => x.Name == _packageName); } }

//        public IList<string> ListViews(string path)
//        {
//            var views = new List<string>();

//            path = normalizePath(path);
//            if (!path.StartsWith(_prefix))
//            {
//                return views;
//            }

//            var virtualPath = getVirtualPath(path);
//            var scope = Path.Combine(_prefix, virtualPath);
//            Package.ForFolder(BottleFiles.WebContentFolder, folder =>
//            {
//                var viewFolderPath = Path.Combine(folder, _virtualFolderRoot, virtualPath);
//                var foundViews = _viewFinder.FindFrom(scope, viewFolderPath);
//                views.AddRange(foundViews);

//            });

//            return views;
//        }
//        public bool HasView(string path)
//        {
//            var file = fileFrom(path);
//            return file != null && file.Exists;
//        }
//        public IViewFile GetViewSource(string path)
//        {
//            var file = fileFrom(path);
//            return file != null && file.Exists ? new FileSystemViewFile(file.FullName) : null;
//        }

//        public PackageViewFolder UseFinder<T>() where T : IViewFinder, new()
//        {
//            _viewFinder = new T();
//            return this;
//        }
//        public static PackageViewFolder Create(string packageName, string virtualFolderRoot, IEnumerable<IPackageInfo> packages)
//        {
//            return new PackageViewFolder(packageName, virtualFolderRoot)
//            {
//                _packages = packages
//            };
//        }

//        private FileInfo fileFrom(string path)
//        {
//            FileInfo fileInfo = null;

//            path = normalizePath(path);
//            if (path.StartsWith(_prefix))
//            {
//                var virtualPath = getVirtualPath(path);
//                Package.ForFolder(BottleFiles.WebContentFolder, folder =>
//                {
//                    var viewPath = Path.Combine(folder, _virtualFolderRoot, virtualPath);
//                    fileInfo = new FileInfo(viewPath);
//                });
//            }

//            return fileInfo;
//        }
//        private string normalizePath(string path)
//        {
//            return path.IsEmpty() ? _prefix : trimPathPrefix(path);
//        }
//        private string getVirtualPath(string path)
//        {
//            var virtualPath = path.Substring(_prefix.Length, path.Length - _prefix.Length);
//            return trimPathPrefix(virtualPath);
//        }
//        private static string trimPathPrefix(string path)
//        {
//            return path.TrimStart(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
//        }
//    }

//    public interface IViewFinder
//    {
//        IEnumerable<string> FindFrom(string virtualPath, string viewFolderPath);
//    }

//    public class DefaultViewFinder : IViewFinder
//    {
//        public IEnumerable<string> FindFrom(string virtualPath, string viewFolderPath)
//        {
//            var views = new List<string>();
//            var viewFolder = new DirectoryInfo(viewFolderPath);

//            if (!viewFolder.Exists)
//            {
//                return views;
//            }

//            viewFolder.GetFiles("*.spark").Each(file =>
//            {
//                var currentDirectory = file.Directory;
//                var viewDirectory = string.Empty;

//                while (currentDirectory != null && currentDirectory.FullName != viewFolder.FullName)
//                {
//                    viewDirectory = Path.Combine(viewDirectory, currentDirectory.Name);
//                    currentDirectory = currentDirectory.Parent;
//                }

//                var viewPath = Path.Combine(virtualPath, viewDirectory, file.Name);

//                views.Add(viewPath);
//            });

//            return views;
//        }
//    }
//}