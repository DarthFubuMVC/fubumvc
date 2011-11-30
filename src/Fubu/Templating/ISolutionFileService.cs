using System;
using System.Collections.Generic;
using System.Text;
using FubuCore;

namespace Fubu.Templating
{
    public interface ISolutionFileService
    {
        void AddProject(string slnFile, CsProj project);
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
    }

    public static class EnvironmentalStringExtensions
    {
        private static readonly string[] Splitters = {
                                                         "\r\n", "\n"
                                                     };
        public static string[] SplitOnNewLine(this string value)
        {
            return value.Split(Splitters, StringSplitOptions.None);
        }
    }
}