using System.Collections.Generic;
using System.Text;
using FubuCore;

namespace Fubu.Templating
{
    public interface ISolutionFileService
    {
        void Save(Sln solution);
    }

    public class Sln
    {
        private readonly IList<CsProj>  _projects = new List<CsProj>();
        private readonly IList<string> _postSolution = new List<string>(); 

        public Sln(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
        public IEnumerable<CsProj> Projects { get { return _projects; } }
        public IEnumerable<string> PostSolutionConfiguration { get { return _postSolution; } }

        public void AddProject(CsProj project)
        {
            _projects.Fill(project);
        }

        public void RegisterPostSolutionConfiguration(string projectGuid, string config)
        {
            var id = "{" + projectGuid + "}";
            _postSolution.Fill("{0}.{1}".ToFormat(id, config));
        }

        public void RegisterPostSolutionConfigurations(string projectGuid, params string[] configs)
        {
            configs.Each(config => RegisterPostSolutionConfiguration(projectGuid, config));
        }
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

            var lines = SplitSolution(solutionContents);
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

        public string[] SplitSolution(string solutionContents)
        {
            return solutionContents.SplitOnNewLine();
        }

        public void Save(Sln solution)
        {
            var solutionContents = _fileSystem.ReadStringFromFile(solution.FileName);
            var replacedContents = new StringBuilder();
            var appended = false;

            var lines = SplitSolution(solutionContents);
            lines.Each(line =>
            {
                if (line.Equals("Global") && !appended)
                {
                    solution
                        .Projects
                        .Each(project =>
                                  {
                                      var projectGuid = "{" + project.ProjectGuid + "}";
                                      var projectType = "Project(\"{" + project.ProjectType + "}\")";
                                      replacedContents.AppendLine("{0} = \"{1}\", \"{2}\", \"{3}\"".ToFormat(projectType,
                                          project.Name, project.RelativePath, projectGuid));
                                      replacedContents.AppendLine("EndProject");
                                  });
                    appended = true;
                }

                replacedContents.AppendLine(line);
            });

            _fileSystem.WriteStringToFile(solution.FileName, replacedContents.ToString());
        }
    }
}