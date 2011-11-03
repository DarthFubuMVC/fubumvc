using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace Serenity.Jasmine
{
    public class SpecificationGraph
    {
        private readonly Cache<string, SpecificationFolder> _packages 
            = new Cache<string, SpecificationFolder>(name => new SpecificationFolder(name));

        public SpecificationGraph(IAssetPipeline pipeline)
        {
            pipeline.AllPackages.Each(AddSpecs);
        }

        public void AddSpecs(PackageAssets package)
        {
            var folder = _packages[package.PackageName];

            var javascriptFiles = package
                .AllFiles()
                .Where(x => x.MimeType == MimeType.Javascript).ToList();


            javascriptFiles
                .Where(Specification.IsSpecification)
                .GroupBy(x => x.ContentFolder())
                .Each(group =>
                {
                    if (!group.Key.IsEmpty())
                    {
                        folder = folder.ChildFolderFor(new SpecPath(group.Key));
                    }

                    folder.AddSpecs(group);
                });

            var specs = AllSpecifications.ToList();
            javascriptFiles.RemoveAll(file => specs.Any(x => x.File == file));
            specs.Each(spec =>
            {
                // Brute force baby!  No elegance needed here.
                // Ten bucks says this is a perf problem down the line
                javascriptFiles.Where(file => spec.DependsOn(file)).Each(file => spec.AddLibrary(file));
            });
        }

        public IEnumerable<Specification> AllSpecifications
        {
            get
            {
                return _packages.OrderBy(x => x.FullName).SelectMany(x => x.AllSpecifications);
            }
        }

        public IEnumerable<SpecificationFolder> Folders
        {
            get
            {
                return _packages;
            }
        }

        public Specification FindSpec(string name)
        {
            return AllSpecifications.FirstOrDefault(x => x.File.Name == name);
        }

        
    }

    public class SpecificationFolder
    {
        private readonly Cache<string, SpecificationFolder> _children;
        private readonly string _name;
        private readonly SpecificationFolder _parent;
        private readonly IList<Specification> _specifications = new List<Specification>();

        public SpecificationFolder(string name)
        {
            _name = name;

            _children = new Cache<string, SpecificationFolder>(path => new SpecificationFolder(path, this));
        }

        public SpecificationFolder(string name, SpecificationFolder parent) : this(name)
        {
            _parent = parent;
        }

        public SpecificationFolder Parent
        {
            get { return _parent; }
        }

        public string FullName
        {
            get
            {
                return _parent == null ? _name : _parent.FullName + "/" + _name;
            }
        }

        public IEnumerable<Specification> Specifications
        {
            get { return _specifications; }
        }

        public IEnumerable<Specification> AllSpecifications
        {
            get
            {
                foreach (var folder in _children)
                {
                    foreach (var spec in folder.AllSpecifications)
                    {
                        yield return spec;
                    }
                }

                foreach (var spec in _specifications)
                {
                    yield return spec;
                }

            }
        }

        public SpecificationFolder ChildFolderFor(string name)
        {
            return ChildFolderFor(new SpecPath(name));
        }

        public SpecificationFolder ChildFolderFor(SpecPath path)
        {
            return path.Parts.Count == 1 
                ? _children[path.TopFolder] 
                : _children[path.TopFolder].ChildFolderFor(path.ChildPath());
        }


        public void AddSpecs(IEnumerable<AssetFile> assetFiles)
        {
            var specs = assetFiles.Select(x => new Specification(x));
            _specifications.AddRange(specs);
        }
    }
}