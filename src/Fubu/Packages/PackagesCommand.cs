using System;
using Bottles;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;

namespace Fubu.Packages
{
    [CommandDescription("Display and modify the state of package zip files in an application folder")]
    public class PackagesCommand : FubuCommand<PackagesInput>
    {
        public override bool Execute(PackagesInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder).ToFullPath();

            Execute(input, new FileSystem());
            return true;
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
                // TODO -- will need to do this for assembly packages as well

                Console.WriteLine("Exploding all the package zip files for the application at " + input.AppFolder);
                exploder.ExplodeAllZipsAndReturnPackageDirectories(input.AppFolder);
            }

            if (input.RemoveAllFlag)
            {
                Console.WriteLine("Removing all package files and directories from the application at " + input.AppFolder);
                new FileSystem().DeleteDirectory(input.AppFolder, "bin", BottleFiles.PackagesFolder);
            }

            exploder.LogPackageState(input.AppFolder);
        }

        public virtual IPackageExploder BuildExploder()
        {
            return new PackageExploder(new ZipFileService(), new PackageExploderLogger(s => Console.WriteLine(s)), new FileSystem());
        }
    }
}