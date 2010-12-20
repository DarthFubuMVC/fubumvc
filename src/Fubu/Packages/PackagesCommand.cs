using System;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu.Packages
{
    public class PackagesInput
    {
        public string AppFolder { get; set; }
        public bool CleanAllFlag { get; set; }
        public bool ExplodeFlag { get; set; }
    }

    [CommandDescription("Access to the state of packages in an application folder")]
    public class PackagesCommand : FubuCommand<PackagesInput>
    {
        public override void Execute(PackagesInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder).ToFullPath();

            Execute(input, new FileSystem());
        }

        public void Execute(PackagesInput input, IFileSystem fileSystem)
        {
            var exploder = BuildExploder();

            if (input.CleanAllFlag)
            {
                Console.WriteLine("Cleaning all exploded packages out of " + input.AppFolder);
                exploder.CleanAll(input.AppFolder);
            }

            if (input.ExplodeFlag)
            {
                Console.WriteLine("Exploding all the package zip files for the application at " + input.AppFolder);
                exploder.ExplodeAll(input.AppFolder);
            }

            exploder.LogPackageState(input.AppFolder);
        }

        public virtual IPackageExploder BuildExploder()
        {
            return new PackageExploder(new ZipFileService(), new PackageExploderLogger(s => Console.WriteLine(s)), new FileSystem());
        }
    }
}