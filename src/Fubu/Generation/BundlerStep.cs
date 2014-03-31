using System;
using System.Diagnostics;
using FubuCore.CommandLine;
using FubuCsProjFile.Templating;
using FubuCsProjFile.Templating.Planning;
using FubuCsProjFile.Templating.Runtime;

namespace Fubu.Generation
{
    public class BundlerStep : ITemplateStep
    {
        public void Alter(TemplatePlan plan)
        {
            if (!RemoteOperations.Enabled)
            {
                plan.Logger.WriteSuccess("Remote operations are disabled.");
                return;
            }

            var bundler = new ProcessStartInfo
            {
                UseShellExecute = !FubuCore.Platform.IsUnix (),
                FileName = "bundle",
                Arguments = "install",
                CreateNoWindow = true,
                WorkingDirectory = plan.Root
            };

            var process = Process.Start(bundler);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                plan.Logger.WriteWarning("bundler install failed!");
            }
            else
            {
                plan.Logger.WriteSuccess("bundler install was successful");
            }
        }

        public override string ToString()
        {
            return "Use Bundler to download new gems";
        }
    }
}