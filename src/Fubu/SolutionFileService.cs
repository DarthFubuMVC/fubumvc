using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FubuCore;

namespace Fubu
{
    public class SolutionModifierTemplateStep : ITemplateStep
    {
        private readonly ISolutionFileService _solutionFileService;
        private readonly ICsProjGatherer _projGatherer;

        public SolutionModifierTemplateStep(ISolutionFileService solutionFileService, ICsProjGatherer projGatherer)
        {
            _solutionFileService = solutionFileService;
            _projGatherer = projGatherer;
        }

        public void Execute(TemplatePlanContext context)
        {
            var solutionFile = context.Input.SolutionFlag;
            var solutionDir = new FileInfo(solutionFile).Directory.FullName;
            _projGatherer
                .GatherProjects(solutionDir)
                .Each(project => _solutionFileService.AddProject(solutionFile, project));
        }
    }

    public interface ISolutionFileService
    {
        void AddProject(string slnFile, CsProj project);
    }

    public class CsProj
    {
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public string ProjectGuid { get; set; }
    }

    public class SolutionFileService : ISolutionFileService
    {
        private readonly IFileSystem _fileSystem;

        public SolutionFileService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void AddProject(string slnFile, CsProj project)
        {
            var solutionContents = _fileSystem.ReadStringFromFile(slnFile);
            var replacedContents = new StringBuilder();
            var appended = false;

            var lines = solutionContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines.Each(line =>
            {
                if (line.Equals("Global") && !appended)
                {
                    var projectGuid = "{" + project.ProjectGuid + "}";
                    var projectType = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\")";
                    replacedContents.AppendLine("{0} = \"{1}\", \"{2}\", \"{3}\"".ToFormat(projectType,
                        project.Name, project.RelativePath, projectGuid));
                    replacedContents.AppendLine("EndProject");
                    appended = true;
                }

                replacedContents.AppendLine(line);
            });

            _fileSystem.WriteStringToFile(slnFile, replacedContents.ToString());
        }
    }

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
                relativePath = Path.Combine(parent.Name, relativePath);
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