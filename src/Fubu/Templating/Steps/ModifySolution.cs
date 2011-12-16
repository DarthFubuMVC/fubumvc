using System.Collections.Generic;
using FubuCore;

namespace Fubu.Templating.Steps
{
    public class ModifySolution : ITemplateStep
    {
        private readonly ISolutionFileService _solutionFileService;
        private readonly ICsProjGatherer _projGatherer;

        private static readonly string[] Configurations = {
"Debug|Any CPU.ActiveCfg = Debug|Any CPU",
"Debug|Any CPU.Build.0 = Debug|Any CPU",
"Debug|Mixed Platforms.ActiveCfg = Debug|Any CPU",
"Debug|Mixed Platforms.Build.0 = Debug|Any CPU",
"Debug|x86.ActiveCfg = Debug|Any CPU",
"Release|Any CPU.ActiveCfg = Release|Any CPU",
"Release|Any CPU.Build.0 = Release|Any CPU",
"Release|Mixed Platforms.ActiveCfg = Release|Any CPU",
"Release|Mixed Platforms.Build.0 = Release|Any CPU",
"Release|x86.ActiveCfg = Release|Any CPU",
"Retail|Any CPU.ActiveCfg = Release|Any CPU",
"Retail|Any CPU.Build.0 = Release|Any CPU",
"Retail|Mixed Platforms.ActiveCfg = Release|Any CPU",
"Retail|Mixed Platforms.Build.0 = Release|Any CPU",
"Retail|x86.ActiveCfg = Release|Any CPU"
                                                          };

        public ModifySolution(ISolutionFileService solutionFileService, ICsProjGatherer projGatherer)
        {
            _solutionFileService = solutionFileService;
            _projGatherer = projGatherer;
        }

        public string Describe(TemplatePlanContext context)
        {
            return "Modify {0} to include project templates".ToFormat(context.Input.SolutionFlag);
        }

        public void Execute(TemplatePlanContext context)
        {
            var solutionFile = context.Input.SolutionFlag;
            var sln = new Sln(solutionFile);
            
            _projGatherer
                .GatherProjects(context.TempDir)
                .Each(project =>
                {
                    sln.AddProject(project);
                    sln.RegisterPostSolutionConfigurations(project.ProjectGuid, Configurations);
                });

            _solutionFileService.Save(sln);
        }
    }
}