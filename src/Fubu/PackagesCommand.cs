using System;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Exploding;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu
{

    [CommandDescription("Display and modify the state of package zip files in an application folder")]
    public class PackagesCommand : FubuCommand<PackagesInput>
    {
        private IFileSystem _system = new FileSystem();

        public override bool Execute(PackagesInput input)
        {
            input.AppFolder = new AliasService().GetFolderForAlias(input.AppFolder).ToFullPath();

            Execute(input, new PackageService(_system));
            return true;
        }

        public void Execute(PackagesInput input, IPackageService service)
        {
            var exploder = BuildExploder();

            if (input.CleanAllFlag)
            {
                service.CleanAllPackages(input.AppFolder);
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
                service.RemoveAllPackages(input.AppFolder);
            }

            // TODO -- this needs to be redone
            //exploder.LogPackageState(input.AppFolder);
        }


        //REVIEW: should this be here?
        public virtual IBottleExploder BuildExploder()
        {
            return BottleExploder.GetPackageExploder(new FileSystem());
        }
    }
}