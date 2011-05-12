using System.ComponentModel;
using Bottles.Deployment.Diagnostics;
using Bottles.Deployment.Parsing;
using Bottles.Deployment.Runtime;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Deployment.Commands
{

    public class ReportInput
    {
        public string Profile { get; set; }

        [Description("Path to where the deployment folder is ~/deployment")]
        public string DeploymentFlag { get; set; }

        public string DeploymentRoot()
        {
            return DeploymentFlag ?? ".".ToFullPath();
        }
    }

    [CommandDescription("Generates Report", Name="report")]
    public class ReportCommand : FubuCommand<ReportInput>
    {
        public override bool Execute(ReportInput input)
        {
            //REVIEW - bah! sh-it code
            var options = new DeploymentOptions(input.Profile);
            var pr = new ProfileReader(new RecipeSorter(), new DeploymentSettings(input.DeploymentRoot()), new FileSystem());
            var plan = pr.Read(options);
            new DiagnosticsReport().Report(plan, "report.html");
            new FileSystem().LaunchBrowser("report.html");
            return true;
        }
    }

}