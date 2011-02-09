using System;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;

namespace Fubu.Packages
{
    [Usage("install", "Install a package zip file to an application")]
    [Usage("uninstall", "Remove a package zip file from an application")]
    [CommandDescription("Install a package zip file to the specified application", Name = "install-pak")]
    public class InstallPackageCommand : FubuCommand<InstallPackageInput>
    {
        public override bool Execute(InstallPackageInput input)
        {
            var applicationFolder = AliasCommand.AliasFolder(input.AppFolder);
            var packageFolder = FileSystem.Combine(applicationFolder, "bin", FubuMvcPackages.FubuPackagesFolder);

            var destinationFileName = FileSystem.Combine(packageFolder, Path.GetFileName(input.PackageFile));
            if (input.UninstallFlag)
            {
                if (File.Exists(destinationFileName))
                {
                    Console.WriteLine("Deleting existing file " + destinationFileName);
                    File.Delete(destinationFileName);
                }
                else
                {
                    Console.WriteLine("File {0} does not exist", destinationFileName);
                }


                return true;
            }

            if (!Directory.Exists(packageFolder))
            {
                Console.WriteLine("Creating folder " + packageFolder);
                Directory.CreateDirectory(packageFolder);
            }


            if (File.Exists(destinationFileName))
            {
                Console.WriteLine("Deleting existing file at " + destinationFileName);
                File.Delete(destinationFileName);
            }

            Console.WriteLine("Copying {0} to {1}", input.PackageFile, packageFolder);

            File.Copy(input.PackageFile, destinationFileName);
            return true;
        }
    }
}