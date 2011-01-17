using System;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;

namespace Fubu.Packages
{

    public class InstallPackageInput
    {
        [Description("The package zip file location to be installed.  If un-installing, just use the zip file name")]
        [RequiredUsage("install", "uninstall")]
        public string PackageFile { get; set; }

        [Description("The physical folder (or valid alias) of the main application")]
        [RequiredUsage("install", "uninstall")]
        public string AppFolder { get; set; }

        [FlagAlias("u")]
        [RequiredUsage("uninstall")]
        [ValidUsage("uninstall")]
        [Description("Uninstalls the named package from an application folder")]
        public bool UninstallFlag { get; set; }
    }

    [Usage("install", "Install a package zip file to an application")]
    [Usage("uninstall", "Remove a package zip file to an application")]
    [CommandDescription("Install a package zip file to the specified application", Name = "install-pak")]
    public class InstallPackageCommand : FubuCommand<InstallPackageInput>
    {
        public override void Execute(InstallPackageInput input)
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


                return;
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
        }
    }


    public class PackagesInput
    {
        [Description("Physical root folder (or valid alias) of the application")]
        public string AppFolder { get; set; }
        
        [Description("Removes all 'exploded' package folders out of the application folder")]
        public bool CleanAllFlag { get; set; }

        [Description("'Explodes' all the zip files underneath <appfolder>/bin/fubu-packages")]
        public bool ExplodeFlag { get; set; }

        [Description("Removes all package zip files and exploded directories from the application folder")]
        public bool RemoveAllFlag { get; set; }
    }

    [CommandDescription("Display and modify the state of package zip files in an application folder")]
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

            if (input.RemoveAllFlag)
            {
                Console.WriteLine("Removing all package files and directories from the application at " + input.AppFolder);
                new FileSystem().DeleteDirectory(input.AppFolder, "bin", FubuMvcPackages.FubuPackagesFolder);
            }

            exploder.LogPackageState(input.AppFolder);
        }

        public virtual IPackageExploder BuildExploder()
        {
            return new PackageExploder(new ZipFileService(), new PackageExploderLogger(s => Console.WriteLine(s)), new FileSystem());
        }
    }
}