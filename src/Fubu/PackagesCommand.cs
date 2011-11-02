using System;
using System.ComponentModel;
using Bottles;
using Bottles.Commands;
using Bottles.Diagnostics;
using Bottles.Exploding;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu
{

    [CommandDescription("Display and modify the state of package zip files in an application folder")]
    public class PackagesCommand : FubuCommand<PackagesInput>
    {
        private IFileSystem _system = new FileSystem();

        public override bool Execute(PackagesInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder).ToFullPath();

            Execute(input, _system);
            return true;
        }

        public void Execute(PackagesInput input, IFileSystem fileSystem)
        {
            var exploder = BuildExploder();

            if (input.CleanAllFlag)
            {
                exploder.CleanAll(input.AppFolder);
            }
            

            if (input.ExplodeFlag)
            {
                // TODO -- will need to do this for assembly packages as well

                Console.WriteLine("Exploding all the package zip files for the application at " + input.AppFolder);
                exploder.ExplodeAllZipsAndReturnPackageDirectories(input.AppFolder, new PackageLog());
            }

            // TODO -- this doesn't work for anything but fubu
            if (input.RemoveAllFlag)
            {
                var packageFolder = BottleFiles.GetApplicationPackagesDirectory(input.AppFolder);
                Console.WriteLine("Removing all package files and directories from the application at " + packageFolder);
                new FileSystem().DeleteDirectory(packageFolder);

                var otherFolder = input.AppFolder.AppendPath("fubu-content");
                Console.WriteLine("Removing all package files and directories from the application at " + otherFolder);
                new FileSystem().DeleteDirectory(otherFolder);
            }

            // TODO -- this needs to be redone
            //exploder.LogPackageState(input.AppFolder);
        }


        //REVIEW: should this be here?
        public virtual IPackageExploder BuildExploder()
        {
            return PackageExploder.GetPackageExploder(new FileSystem());
        }
    }
}