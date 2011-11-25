using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FubuCore;

namespace Fubu.Templating
{
    public interface ICsProjGatherer
    {
        IEnumerable<CsProj> GatherProjects(string directory);
    }

    public class CsProjGatherer : ICsProjGatherer
    {
        private readonly IFileSystem _fileSystem;

        private static readonly FileSet CsProjFileSet = new FileSet
        {
            DeepSearch = true,
            Include = "*.csproj",
            Exclude = "*.exe;*.dll"
        };

        public CsProjGatherer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<CsProj> GatherProjects(string directory)
        {
            return _fileSystem
                .FindFiles(directory, CsProjFileSet)
                .Select(file => CreateProj(directory, file));
        }

        public CsProj CreateProj(string directory, string projFile)
        {
            var proj = new CsProj
            {
                RelativePath = FindRelativePath(directory, projFile)
            };
            VisitProj(projFile, proj);
            return proj;
        }

        public string FindRelativePath(string directory, string projFile)
        {
            var dirName = _fileSystem.GetFileName(directory);
            var relativePath = _fileSystem.GetFileName(projFile);
            var file = new FileInfo(projFile);
            var parent = file.Directory;
            while (parent != null && parent.Name.ToLower() != dirName.ToLower())
            {
                // these relative paths are always this way in sln files -- Mono seems to do just fine
                relativePath = parent.Name + "\\" + relativePath;
                parent = parent.Parent;
            }

            return relativePath;
        }

        public void VisitProj(string projFile, CsProj project)
        {
            var doc = XDocument.Load(projFile, LoadOptions.None);
            var root = doc
                .Elements()
                .First(e => e.Name.LocalName == "Project")
                .Elements()
                .First(e => e.Name.LocalName == "PropertyGroup");

            project.ProjectGuid = root
                .Elements()
                .First(e => e.Name.LocalName == "ProjectGuid")
                .Value
                .TrimStart('{')
                .TrimEnd('}');

            project.Name = root
                .Elements()
                .First(e => e.Name.LocalName == "AssemblyName")
                .Value;
        }
    }
}