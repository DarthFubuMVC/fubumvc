using System;
using System.ComponentModel;
using System.IO;
using FubuCore.CommandLine;

namespace Bottles.Commands
{
    public class FolderInput
    {
        [Description("Physical folder (or valid alias) of the web application")]
        public string AppFolder { get; set; }
    }

    [CommandDescription("Restarts a web application by 'touching' the web.config file")]
    public class RestartCommand : FubuCommand<FolderInput>
    {
        public override bool Execute(FolderInput input)
        {
            var folder = AliasCommand.AliasFolder(input.AppFolder);

            Restart(folder);
            return true;
        }

        public static void Restart(string folder)
        {
            var configFile = Path.Combine(folder, "web.config");
            File.SetLastWriteTimeUtc(configFile, DateTime.UtcNow);
        }
    }
}