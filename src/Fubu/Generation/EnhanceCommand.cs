using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using FubuCore.CommandLine;
using FubuCore;
using FubuCsProjFile;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Graph;

namespace Fubu.Generation
{
    public class EnhanceInput
    {
        [Description("Name of the project to enhance")]
        public string Project { get; set; }

        [Description("The options to apply")]
        public IEnumerable<string> Options { get; set; }

        [Description("List all the valid options")]
        public bool ListFlag { get; set; }
    }

    [CommandDescription("Enhances an existing project by applying infrastructure templates to an existing project")]
    public class EnhanceCommand : FubuCommand<EnhanceInput>
    {
        public override bool Execute(EnhanceInput input)
        {
            var solutionFile = SolutionFinder.FindSolutionFile();
            if (solutionFile == null) return false;

            var request = BuildTemplateRequest(input, solutionFile);

            var plan = Templating.BuildPlan(request);
            plan.Solution = Solution.LoadFrom(solutionFile);

            Templating.ExecutePlan(plan);

            return true;
        }

        public static TemplateRequest BuildTemplateRequest(EnhanceInput input, string solutionFile)
        {
            var request = new TemplateRequest
            {
                RootDirectory = Environment.CurrentDirectory.ToFullPath(),
                SolutionName = Path.GetFileNameWithoutExtension(solutionFile)
            };

            var projectRequest = new ProjectRequest(input.Project, "baseline");
            request.AddProjectRequest(projectRequest);
            input.Options.Each(o => projectRequest.Alterations.Add(o));
            return request;
        }
    }
}