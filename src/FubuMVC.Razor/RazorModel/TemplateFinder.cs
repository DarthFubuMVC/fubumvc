﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using FubuCore.Util;
using FubuMVC.Core.Packaging;
using FubuMVC.Razor.RazorModel.Scanning;

namespace FubuMVC.Razor.RazorModel
{
    public interface ITemplateFinder
    {
        IEnumerable<ITemplate> FindInHost();
        IEnumerable<ITemplate> FindInPackages();
    }

    public class TemplateFinder : ITemplateFinder
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
                // TODO -- re-evaluate this
                return _hostPath ?? FubuMvcPackageFacility.GetApplicationPath();
            } 
            set { _hostPath = value; }
        }

        public IEnumerable<ITemplate> FindInHost()
        {
            var templates = new List<ITemplate>();
            var root = new RazorRoot
            {
                Origin = FubuRazorConstants.HostOrigin, 
                Path = HostPath
            };

            var request = buildRequest(templates, root);
            _hostExcludes.Do(request);
            
            _fileScanner.Scan(request);
            
            return templates;
        }

        public IEnumerable<ITemplate> FindInPackages()
        {
            var templates = new List<ITemplate>();
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

        private static IEnumerable<RazorRoot> packageRoots(IEnumerable<IPackageInfo> packages)
        {
            var packageRoots = new List<RazorRoot>();
            foreach (var package in packages)
            {
                var pack = package;
                package.ForFolder(BottleFiles.WebContentFolder, path =>
                {
                    var root = new RazorRoot
                    {
                        Origin = pack.Name,
                        Path = path
                    };

                    packageRoots.Add(root);
                });
            }
            return packageRoots;
        }

        private ScanRequest buildRequest(ICollection<ITemplate> templates, params RazorRoot[] RazorRoots)
        {
            var request = new ScanRequest();
            _requestConfig.Do(request);
            
            RazorRoots.Each(r => request.AddRoot(r.Path));
            request.AddHandler(fileFound =>
            {
                var origin = RazorRoots.First(x => x.Path == fileFound.Root).Origin;
                var RazorFile = new Template(fileFound.Path, fileFound.Root, origin);                
                templates.Add(RazorFile);
            });

            return request;
        }
    }      
}