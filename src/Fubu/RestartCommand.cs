using System;
using System.IO;
using FubuCore.CommandLine;

namespace Fubu
{
    public class FolderInput
    {
        public string FolderName { get; set; }
    }

    [CommandDescription("Restarts a web application by 'touching' the web.config file")]
    public class RestartCommand : FubuCommand<FolderInput>
    {
        public override void Execute(FolderInput input)
        {
            var configFile = Path.Combine(input.FolderName, "web.config");
            File.SetLastWriteTimeUtc(configFile, DateTime.UtcNow);
        }
    }
}