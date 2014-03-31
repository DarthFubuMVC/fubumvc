using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Descriptions;
using FubuCsProjFile;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Runtime;

namespace Fubu.Generation
{
    public class AddInput
    {
        private bool _noTestsFlag;

        [Description("The name of the new project")]
        public string ProjectName { get; set; }

        [Description("Project profile.  Use the --list flag to see the valid options")]
        public string Profile { get; set; }

        [Description("Specify the solution file name if there is more than one in this code tree")]
        public string SolutionFlag { get; set; }

        [Description("Used in many templates as a prefix for generated classes")]
        public string ShortNameFlag { get; set; }

        [Description("Do not generate a matching testing project.  Boo!")]
        [FlagAlias("no-tests", 'n')]
        public bool NoTestsFlag
        {
            get
            {
                if (Profile.EqualsIgnoreCase("fubudocs")) return true;
                
                
                return _noTestsFlag;
            }
            set { _noTestsFlag = value; }
        }

        [Description("List all the possible project types and their valid options")]
        public bool ListFlag { get; set; }


        [Description("Extra options for the new application")]
        public IEnumerable<string> OptionsFlag { get; set; }

        public string DetermineShortName()
        {
            return ShortNameFlag.IsEmpty()
                ? ProjectName.Split('.').Skip(1).Join("")
                : ShortNameFlag;
        }

        [Description("Specify the .Net version. 'v4.0' is the default. Options are 'v4.0' or 'v4.5'")]
        public string DotNetFlag { get; set; }


        public TemplateChoices ToChoices()
        {
            return new TemplateChoices
            {
                Category = "add",
                ProjectName = ProjectName,
                ProjectType = Profile,
                Options = OptionsFlag
            };
        }

        public void AssertValid()
        {
            if (Profile.EqualsIgnoreCase("fubudocs") && !ProjectName.EndsWith(".Docs"))
            {
                throw new ApplicationException("Any FubuDocs project must be named with the '.Docs' suffix");
            }
        }
    }

    [CommandDescription("Adds new projects to an existing solution")]
    public class AddCommand : FubuCommand<AddInput>
    {
        public AddCommand()
        {
            Usage("default").Arguments(x => x.ProjectName, x => x.Profile);
            Usage("list").Arguments().ValidFlags(x => x.ListFlag);
        }

        public override bool Execute(AddInput input)
        {
            input.AssertValid();

            if (input.ListFlag)
            {
                Templating.Library.Graph.FindCategory("add").WriteDescriptionToConsole();
                return true;
            }

            var solutionFile = input.SolutionFlag ?? SolutionFinder.FindSolutionFile();

            if (solutionFile.IsEmpty()) return false;

            try
            {
                var request = BuildTemplateRequest(input, solutionFile);
                var plan = Templating.BuildPlan(request);
                Templating.ExecutePlan(plan);
            }
            catch (Exception)
            {
                Console.WriteLine("Template planning failed.  The valid options for this command are:");
                Templating.Library.Graph.FindCategory("add").WriteDescriptionToConsole();
                Console.WriteLine();
                Console.WriteLine();
                throw;
            }

            return true;
        }

        public static TemplateRequest BuildTemplateRequest(AddInput input, string solutionFile)
        {
            var request = new TemplateRequest
            {
                RootDirectory = Environment.CurrentDirectory,
                SolutionName = solutionFile
            };

            var shortName = input.DetermineShortName();
            request.Substitutions.Set(ProjectPlan.SHORT_NAME, shortName);

            var choices = input.ToChoices();

            var project = Templating.Library.Graph.BuildProjectRequest(choices);
            if (input.DotNetFlag.IsNotEmpty())
            {
                project.Version = input.DotNetFlag;
            }

            project.Version = project.Version ?? DotNetVersion.V40;



            request.AddProjectRequest(project);
            if (!input.NoTestsFlag)
            {
                request.AddMatchingTestingProject(project);
            }

            return request;
        }
    }
}