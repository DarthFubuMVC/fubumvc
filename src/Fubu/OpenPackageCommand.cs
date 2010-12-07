using System;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu
{
    public class OpenPackageInput
    {
        public string PackageFolder { get; set; }
    }

    [CommandDescription("Opens the requested package manifest file")]
    public class OpenPackageCommand : FubuCommand<OpenPackageInput>
    {
        public override void Execute(OpenPackageInput input)
        {
            var system = new FileSystem();
            var folder = AliasCommand.AliasFolder(input.PackageFolder);

            system.OpenInNotepad(folder, PackageManifest.FILE);
        }
    }
}