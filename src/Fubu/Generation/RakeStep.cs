using System.Diagnostics;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;

namespace Fubu.Generation
{
    public class RakeStep : ITemplateStep
    {
        public void Alter(TemplatePlan plan)
        {
            if (!RemoteOperations.Enabled)
            {
                plan.Logger.WriteSuccess("Remote operations are disabled.");
                return;
            }

            var rake = new ProcessStartInfo
            {
                UseShellExecute = !FubuCore.Platform.IsUnix (),
                FileName = "rake",
                CreateNoWindow = true,
                WorkingDirectory = plan.Root
            };

            Process process = Process.Start(rake);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                plan.Logger.WriteWarning("rake script failed!");
            }
            else
            {
                plan.Logger.WriteSuccess("rake succeeded");
            }
        }

        public override string ToString()
        {
            return "Run the rake script";
        }
    }
}