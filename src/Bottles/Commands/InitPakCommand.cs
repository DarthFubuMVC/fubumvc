using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Commands
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

        [Description("There is no web content to include")]
        [FlagAlias("noweb")]
        public bool NoWebContentFlag { get; set; }
    }

    [CommandDescription("Initialize a package manifest", Name = "init-pak")]
    public class InitPakCommand : FubuCommand<InitPakInput>
    {
        public override bool Execute(InitPakInput input)
        {
            new AliasCommand().Execute(new AliasInput{
                Folder = input.Folder,
                Name = input.AliasFlag ?? input.Name.ToLower()
            });

            Execute(input, new FileSystem());

            return true;
        }

        public void Execute(InitPakInput input, IFileSystem fileSystem)
        {
            var assemblyName = Path.GetFileName(input.Folder);

            var manifest = new PackageManifest{
                Name = input.Name
            };

            if (input.NoWebContentFlag)
                manifest.ContentFileSet = new FileSet(){DeepSearch = false, Include="*.config"};

            manifest.AddAssembly(assemblyName);

			if(!fileSystem.FileExists(FileSystem.Combine(input.Folder, PackageManifest.FILE)))
			{
				fileSystem.PersistToFile(manifest, input.Folder, PackageManifest.FILE);
			}
            

            if (input.NotepadFlag)
            {
                fileSystem.LaunchEditor(input.Folder, PackageManifest.FILE);
            }
        }
    }
}