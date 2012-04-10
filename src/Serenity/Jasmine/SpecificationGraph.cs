using System.Collections.Generic;
using System.Linq;
using Bottles.PackageLoaders.Assemblies;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace Serenity.Jasmine
{
    public class SpecificationGraph : ISpecNode
    {
        private readonly Cache<string, SpecificationFolder> _packages
            = new Cache<string, SpecificationFolder>(name => new SpecificationFolder(name));

        public SpecificationGraph(IAssetPipeline pipeline)
        {
            pipeline.AllPackages.Where(IsSpecPackage).Each(AddSpecs);

            _packages.Each(x => x.ApplyHelpers());
        }

        public IEnumerable<SpecificationFolder> Folders
        {
            get { return _packages; }
        }

        public SpecPath Path()
        {
            return new SpecPath(new List<string>());
        }

        public IEnumerable<Specification> AllSpecifications
        {
            get { return _packages.OrderBy(x => x.FullName).SelectMany(x => x.AllSpecifications); }
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

        public string FullName
        {
            get { return string.Empty; }
        }

        public IEnumerable<ISpecNode> ImmediateChildren
        {
            get
            {
                foreach (var package in _packages)
                {
                    yield return package;
                }
                ;
            }
        }

        public string TreeClass
        {
            get { return "folder"; }
        }

        public ISpecNode Parent()
        {
            return null;
        }

        public void AcceptVisitor(ISpecVisitor visitor)
        {
            visitor.Graph(this);
        }

        public static bool IsSpecPackage(PackageAssets package)
        {
            if (package.PackageName == "application") return false;

            if (package.PackageName == AssemblyPackageInfo.CreateFor(typeof (SpecificationGraph).Assembly).Name)
                return false;

            return true;
        }

        public void AddSpecs(PackageAssets package)
        {
            var packageFolder = _packages[package.PackageName];

            var javascriptFiles = package
                .AllFiles()
                .Where(x => x.MimeType == MimeType.Javascript).ToList();

            

            addSpecs(javascriptFiles, packageFolder);
            associateSpecs(javascriptFiles);

            associateHtmlFiles(package, packageFolder);
        }

        private static void associateHtmlFiles(PackageAssets package, SpecificationFolder packageFolder)
        {
            var htmlFiles = package.AllFiles().Where(x => x.MimeType == MimeType.Html).ToList();
            packageFolder.AllSpecifications.Each(x => x.SelectHtmlFiles(htmlFiles));
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
            return AllSpecifications.FirstOrDefault(x => x.FullName == name);
        }

        public Specification FindSpecByLibraryName(string name)
        {
            return AllSpecifications.FirstOrDefault(x => x.LibraryName == name);
        }

        public ISpecNode FindSpecNode(SpecPath path)
        {
            if (path.FullName.IsEmpty()) return this;

            var fullName = path.FullName;
            return AllNodes.FirstOrDefault(x => x.FullName == fullName);
        }

        public static string DetermineSpecContentFolder(AssetFile file)
        {
            var contentFolder = file.ContentFolder();
            var list = contentFolder.Split('/').ToList();
            list.RemoveAll(x => x == "specs");

            return list.Join("/");
        }
    }
}