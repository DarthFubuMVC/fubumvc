using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuCsProjFile;
using FubuCsProjFile.Templating.Graph;
using FubuCsProjFile.Templating.Runtime;

namespace Fubu.Generation
{
    public class NewCommandInput
    {
        private static readonly IDictionary<FeedChoice, string> _rippleTemplates = new Dictionary<FeedChoice, string>
        {
            {FeedChoice.Edge, "edge-ripple"},
            {FeedChoice.FloatingEdge, "floating-ripple"},
            {FeedChoice.PublicOnly, "public-ripple"}
        };

        private FeedChoice _rippleFlag = FeedChoice.PublicOnly;

        public NewCommandInput()
        {
            Profile = "web-app";
            VersionFlag = Solution.VS2012;
        }

        [Description("Name of the solution and the root folder without an extension")]
        public string SolutionName { get; set; }

        [Description("List all the possible project types and their valid options")]
        public bool ListFlag { get; set; }

        [Description("Only list a preview of the template plan, but do not execute the plan")]
        public bool PreviewFlag { get; set; }

        [Description("Use edge nugets from the Fubu TeamCity feed.  Equivalent to --ripple Edge")]
        public bool EdgeFlag
        {
            get
            {
                return RippleFlag == FeedChoice.Edge;
            }
            set
            {
                RippleFlag = FeedChoice.Edge;
            }
        }

        [Description("Use floating edge nugets from the Fubu TeamCity feed.  Equivalent to --ripple FloatingEdge")]
        public bool FloatingFlag
        {
            get
            {
                return RippleFlag == FeedChoice.Edge;
            }
            set
            {
                RippleFlag = FeedChoice.FloatingEdge;
            }
        }

        [Description(
            "Choose a ripple configuration for only public Nuget feeds, including the Fubu TeamCity feed, or 'floating' on the Fubu edge"
            )]
        public FeedChoice RippleFlag
        {
            get { return _rippleFlag; }
            set { _rippleFlag = value; }
        }

        [Description("Used in many templates as a prefix for generated classes")]
        public string ShortNameFlag { get; set; }

        [Description("Clean out any existing contents of the target folder before running the templates")]
        public bool CleanFlag { get; set; }

        [Description("Do not generate a matching testing project.  Boo!")]
        [FlagAlias("no-tests", 'n')]
        public bool NoTestsFlag { get; set; }

        [Description("Visual Studio version. 'VS2012' is the default. Options are 'VS2010', 'VS2012', or 'VS2013'")]
        public string VersionFlag { get; set; }

        [Description("Specify the .Net version. 'v4.0' is the default. Options are 'v4.0' or 'v4.5'")]
        [FlagAlias("dot-net")]
        public string DotNetFlag { get; set; }

        public string SolutionDirectory()
        {
            return Environment.CurrentDirectory.AppendPath(SolutionName);
        }

        public TemplateRequest CreateRequestForSolution()
        {
            var request = new TemplateRequest
            {
                RootDirectory = SolutionDirectory(),
                SolutionName = SolutionName,
                Version = VersionFlag
            };

            request.AddTemplate("baseline");

            request.AddTemplate(_rippleTemplates[RippleFlag]);

            determineShortName(request);

            if (!Profile.EqualsIgnoreCase("empty"))
            {
                var choices = ToTemplateChoices();
                var project = Templating.Library.Graph.BuildProjectRequest(choices);
                if (DotNetFlag.IsNotEmpty())
                {
                    project.Version = DotNetFlag;
                }

                project.Version = project.Version ?? DotNetVersion.V40;

                request.AddProjectRequest(project);
                if (!NoTestsFlag)
                {
                    request.AddMatchingTestingProject(project);
                }
            }

            return request;
        }

        public TemplateChoices ToTemplateChoices()
        {
            return new TemplateChoices
            {
                Category = "new",
                ProjectName = SolutionName,
                ProjectType = Profile,
                Options = OptionsFlag
            };
        }

        private void determineShortName(TemplateRequest request)
        {
            var shortName = ShortNameFlag.IsEmpty()
                ? SolutionName.Split('.').Skip(1).Join("")
                : ShortNameFlag;

            request.Substitutions.Set(ProjectPlan.SHORT_NAME, shortName);
        }

        [Description("Project profile.  Use the --list flag to see the valid options")]
        public string Profile { get; set; }

        [Description("Ignore the presence of existing files")]
        public bool IgnoreFlag { get; set; }


        [Description("Extra options for the new application")]
        public IEnumerable<string> OptionsFlag { get; set; }

    }

    public enum FeedChoice
    {
        FloatingEdge,
        Edge,
        PublicOnly
    }
}