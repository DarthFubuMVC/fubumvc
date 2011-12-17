using System.Collections.Generic;
using System.Linq;
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
            _postSolution.Fill("\t\t{0}.{1}".ToFormat(id, config));
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

        public string[] SplitSolution(string solutionContents)
        {
            return solutionContents.SplitOnNewLine();
        }

        public void Save(Sln solution)
        {
            var solutionContents = _fileSystem.ReadStringFromFile(solution.FileName);
            var solutionBuilder = new StringBuilder();
            var modifiers = new List<ISolutionFileModifier>
                                {
                                    new AddProjectsModifier(solution),
                                    new AddConfigurationsModifier(solution),
                                    new AppendLineModifier()
                                };

            var lines = SplitSolution(solutionContents);
            lines.Each(line =>
                           {
                               var filteredModifiers = modifiers.Where(m => m.Matches(line));
                               foreach(var m in filteredModifiers)
                               {
                                   if(!m.Modify(line, solutionBuilder))
                                   {
                                       break;
                                   }
                               }
                           });

            _fileSystem.WriteStringToFile(solution.FileName, solutionBuilder.ToString());
        }
    }

    public interface ISolutionFileModifier
    {
        bool Matches(string line);
        bool Modify(string line, StringBuilder builder);
    }

    public class AppendLineModifier : ISolutionFileModifier
    {
        public bool Matches(string line)
        {
            return true;
        }

        public bool Modify(string line, StringBuilder builder)
        {
            builder.AppendLine(line);
            return true;
        }
    }

    public class AddProjectsModifier : ISolutionFileModifier
    {
        private bool _appended;
        private readonly Sln _solution;

        public AddProjectsModifier(Sln solution)
        {
            _solution = solution;
        }

        public bool Matches(string line)
        {
            return line.Equals("Global") && !_appended;
        }

        public bool Modify(string line, StringBuilder builder)
        {
            _solution
                .Projects
                .Each(project =>
                          {
                              var projectGuid = "{" + project.ProjectGuid + "}";
                              var projectType = "Project(\"{" + project.ProjectType + "}\")";
                              builder.AppendLine("{0} = \"{1}\", \"{2}\", \"{3}\"".ToFormat(projectType,
                                                                                            project.Name,
                                                                                            project.RelativePath,
                                                                                            projectGuid));
                              builder.AppendLine("EndProject");
                          });
            
            _appended = true;
            return true;
        }
    }

    public class AddConfigurationsModifier : ISolutionFileModifier
    {
        private readonly Sln _solution;

        public AddConfigurationsModifier(Sln solution)
        {
            _solution = solution;
        }

        public bool Matches(string line)
        {
            return line.EndsWith("GlobalSection(ProjectConfigurationPlatforms) = postSolution");
        }

        public bool Modify(string line, StringBuilder builder)
        {
            builder.AppendLine(line);
            _solution
                .PostSolutionConfiguration
                .Each(config => builder.AppendLine(config));
            return false;
        }
    }
}