using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu
{
    public class InitPakInput
    {
        public string Folder { get; set; }
        public string Name { get; set; }
        public string AliasFlag { get; set; }
        public bool NotepadFlag { get; set; }
    }

    [CommandDescription("initialize a package manifest", Name = "init-pak")]
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
                fileSystem.OpenInNotepad(input.Folder, PackageManifest.FILE);
            }
        }
    }
}