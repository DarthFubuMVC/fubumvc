using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace Fubu.Templating.Steps
{
    public class ModifySolution : ITemplateStep
    {
        private readonly ISolutionFileService _solutionFileService;
        private readonly ICsProjGatherer _projGatherer;

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
            var solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionFile));
            _projGatherer
                .GatherProjects(solutionDir)
                .Each(project => _solutionFileService.AddProject(solutionFile, project));
        }
    }
}