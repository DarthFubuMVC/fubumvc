using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using FubuCore.Util;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.View.Model.Scanning;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateFinder<T> where T : ITemplateFile, new()
    {
        IEnumerable<T> FindInHost();
        IEnumerable<T> FindInPackages();
    }

    public class TemplateFinder<T> : ITemplateFinder<T> where T : ITemplateFile, new()
    {
        private readonly IFileScanner _fileScanner;
        private readonly IEnumerable<IPackageInfo> _packages;
        private CompositeAction<ScanRequest> _requestConfig;
        private CompositeAction<ScanRequest> _hostExcludes;
        private string _hostPath;
        
        public TemplateFinder() : this(new FileScanner(), PackageRegistry.Packages) { }
        public TemplateFinder(IFileScanner fileScanner, IEnumerable<IPackageInfo> packages)
        {
            _fileScanner = fileScanner;
            _packages = packages;
            _requestConfig = new CompositeAction<ScanRequest>();
            _hostExcludes = new CompositeAction<ScanRequest>();
        }

        public string HostPath
        {
            get
            {
                // TODO -- re-evaluate this.  Really needs to be evaluated at the last moment.
                return _hostPath ?? FubuMvcPackageFacility.GetApplicationPath();
            } 
            set { _hostPath = value; }
        }

        public IEnumerable<T> FindInHost()
        {
            var templates = new List<T>();
            var root = new TemplateRoot
            {
                Origin = TemplateConstants.HostOrigin, 
                Path = HostPath
            };

            var request = buildRequest(templates, root);
            _hostExcludes.Do(request);
            
            _fileScanner.Scan(request);
            
            return templates;
        }

        public IEnumerable<T> FindInPackages()
        {
            var templates = new List<T>();
            var roots = packageRoots(_packages).ToArray();
            var request = buildRequest(templates, roots);
            
            _fileScanner.Scan(request);
            
            return templates;
        }

        public void IncludeFile(string filter)
        {
            _requestConfig += r => r.Include(filter);
        }

        public void ExcludeHostDirectory(string path)
        {
            _hostExcludes += r => r.ExcludeDirectory(Path.Combine(HostPath, path));
        }

        public void ExcludeHostDirectory(params string[] parts)
        {
            excludeDirectory(Path.Combine(HostPath, Path.Combine(parts)));
        }

        private void excludeDirectory(string path)
        {
            _hostExcludes += r => r.ExcludeDirectory(path);
        }

        private static IEnumerable<TemplateRoot> packageRoots(IEnumerable<IPackageInfo> packages)
        {
            var packageRoots = new List<TemplateRoot>();
            foreach (var package in packages)
            {
                var pack = package;
                package.ForFolder(BottleFiles.WebContentFolder, path =>
                {
                    var root = new TemplateRoot
                    {
                        Origin = pack.Name,
                        Path = path
                    };

                    packageRoots.Add(root);
                });
            }
            return packageRoots;
        }

        private ScanRequest buildRequest(ICollection<T> templates, params TemplateRoot[] templateRoots)
        {
            var request = new ScanRequest();
            _requestConfig.Do(request);
            
            templateRoots.Each(r => request.AddRoot(r.Path));
            request.AddHandler(fileFound =>
            {
                var origin = templateRoots.First(x => x.Path == fileFound.Root).Origin;
                var templateFile = new T {FilePath = fileFound.Path, RootPath = fileFound.Root, Origin = origin};
                templates.Add(templateFile);
            });

            return request;
        }
    }      
}