using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
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

        public IEnumerable<Specification> AllSpecifications
        {
            get { return _packages.OrderBy(x => x.FullName).SelectMany(x => x.AllSpecifications); }
        }

        public IEnumerable<SpecificationFolder> Folders
        {
            get { return _packages; }
        }

        public void AddSpecs(PackageAssets package)
        {
            var packageFolder = _packages[package.PackageName];

            var javascriptFiles = package
                .AllFiles()
                .Where(x => x.MimeType == MimeType.Javascript).ToList();


            addSpecs(javascriptFiles, packageFolder);
            associateSpecs(javascriptFiles);
        }

        private void associateSpecs(List<AssetFile> javascriptFiles)
        {
            var specs = AllSpecifications.ToList();
            javascriptFiles.RemoveAll(file => specs.Any(x => x.File == file));
            specs.Each(spec =>
            {
                // Brute force baby!  No elegance needed here.
                // Ten bucks says this is a perf problem down the line
                javascriptFiles.Where(spec.DependsOn).Each(spec.AddLibrary);
            });
        }

        private static void addSpecs(IEnumerable<AssetFile> javascriptFiles, SpecificationFolder packageFolder)
        {
            javascriptFiles
                .Where(Specification.IsSpecification)
                .GroupBy(DetermineSpecContentFolder)
                .Each(group =>
                {
                    var folder = packageFolder;

                    if (!group.Key.IsEmpty())
                    {
                        folder = packageFolder.ChildFolderFor(new SpecPath(group.Key));
                    }

                    folder.AddSpecs(group);
                });
        }

        public Specification FindSpecByFullName(string name)
        {
            return AllSpecifications.FirstOrDefault(x => x.File.Name == name);
        }

        public Specification FindSpecByLibraryName(string name)
        {
            return AllSpecifications.FirstOrDefault(x => x.LibraryName == name);
        }

        public ISpecNode FindSpecs(SpecPath path)
        {
            string fullName = path.FullName;
            return AllNodes.FirstOrDefault(x => x.FullName == fullName);
        }

        public static string DetermineSpecContentFolder(AssetFile file)
        {
            var contentFolder = file.ContentFolder();
            var list = contentFolder.Split('/').ToList();
            list.RemoveAll(x => x == "specs");

            return list.Join("/");
        }

        public IEnumerable<ISpecNode> AllNodes
        {
            get
            {
                foreach (var package in _packages)
                {
                    yield return package;

                    foreach (var node in package.AllNodes)
                    {
                        yield return node;
                    }
                }
            }
        }
    }
}