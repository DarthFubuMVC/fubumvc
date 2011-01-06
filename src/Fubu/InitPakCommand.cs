using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu
{
    public class InitPakInput
    {
        [Description("The physical folder of the new package")]
        public string Folder { get; set; }

        [Description("The name of the new package")]
        public string Name { get; set; }

        [Description("Creates a folder alias for the package folder.  Equivalent to fubu alias <folder> <alias>")]
        public string AliasFlag { get; set; }

        [Description("Opens the package manifest file in notepad")]
        public bool NotepadFlag { get; set; }
    }

    [CommandDescription("Initialize a package manifest", Name = "init-pak")]
    public class InitPakCommand : FubuCommand<InitPakInput>
    {
        public override void Execute(InitPakInput input)
        {
            new AliasCommand().Execute(new AliasInput{
                Folder = input.Folder,
                Name = input.AliasFlag ?? input.Name.ToLower()
            });

            Execute(input, new FileSystem());
        }

        public void Execute(InitPakInput input, IFileSystem fileSystem)
        {
            var assemblyName = Path.GetFileName(input.Folder);

            var manifest = new PackageManifest{
                Name = input.Name,
                Assemblies = assemblyName
            };

            fileSystem.PersistToFile(manifest, input.Folder, PackageManifest.FILE);

            if (input.NotepadFlag)
            {
                fileSystem.LaunchEditor(input.Folder, PackageManifest.FILE);
            }
        }
    }
}