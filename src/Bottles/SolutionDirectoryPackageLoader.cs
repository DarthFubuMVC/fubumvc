using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles
{
    public class SolutionDirectoryPackageLoader : IPackageLoader
    {
        private readonly string _sourceRoot;

        public SolutionDirectoryPackageLoader(string sourceRoot)
        {
            _sourceRoot = sourceRoot;
        }

        public IEnumerable<IPackageInfo> Load()
        {
            var fileSystem = new FileSystem();
            var manifestFileSpec = new FileSet { Include = PackageManifest.FILE, DeepSearch = true };
            var manifestReader = new PackageManifestReader(_sourceRoot, fileSystem, folder => folder);
            
            //how can i 'where' the manifests
               

            var pis = fileSystem.FileNamesFor(manifestFileSpec, _sourceRoot)
                .Select(Path.GetDirectoryName)
                .Select(manifestReader.LoadFromFolder);

            var filtered = pis.Where(pi=>PackageRole.Module.Equals(pi.Role));

            ConsoleWriter.PrintHorizontalLine();
            ConsoleWriter.Write("Solution Package Loader found:");
            filtered.Each(p=>ConsoleWriter.Write("  {0}", p.Name));
            ConsoleWriter.PrintHorizontalLine();

            return filtered;
        }
    }
}