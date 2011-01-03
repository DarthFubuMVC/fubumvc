using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu
{




    public class LinkInput
    {
        [Description("The physical folder (or valid alias) of the main application")]
        [RequiredUsage("list", "create", "remove", "clean")]
        public string AppFolder { get; set; }

        [Description("The physical folder (or valid alias) of a package")]
        [RequiredUsage("create", "remove")]
        public string PackageFolder { get; set; }

        [Description("Remove the package folder link from the application")]
        [RequiredUsage("remove")]
        [ValidUsage("remove")]
        public bool RemoveFlag { get; set; }

        [Description("Remove all links from an application manifest file")]
        [ValidUsage("clean")]
        public bool CleanAllFlag { get; set; }

        [Description("Restarts the application -- fubu restart <appfolder>")]
        public bool RestartFlag { get; set; }

        public string RelativePathOfPackage()
        {
            var pkg = Path.GetFullPath(PackageFolder);
            var app = Path.GetFullPath(AppFolder);

            return pkg.PathRelativeTo(app);
        }
    }

    [Usage("list", "List the current links for the application")]
    [Usage("create", "Create a new link for the application to the package")]
    [Usage("remove", "Remove any existing link for the application to the package")]
    [Usage("clean", "Remove any and all existing links from the application to any package folder")]
    [CommandDescription("Links a package folder to an application folder in development mode")]
    public class LinkCommand : FubuCommand<LinkInput>
    {
        public override void Execute(LinkInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder);
            input.PackageFolder = AliasCommand.AliasFolder(input.PackageFolder);


            Execute(input, new FileSystem());
        }

        private void Execute(LinkInput input, FileSystem fileSystem)
        {
            var manifest = fileSystem.LoadApplicationManifestFrom(input.AppFolder);

            if (input.CleanAllFlag && fileSystem.FileExists(input.AppFolder, ApplicationManifest.FILE))
            {
                manifest.RemoveAllLinkedFolders();

                persist(input, manifest, fileSystem);

                Console.WriteLine("Removed all package links from the manifest file for " + input.AppFolder);

                listCurrentLinks(input, manifest);
                
                return;
            }

            

            if (input.PackageFolder.IsNotEmpty())
            {
                updateManifest(input, fileSystem, manifest);
            }
            else
            {
                listCurrentLinks(input, manifest);
            }

            if (input.RestartFlag)
            {
                RestartCommand.Restart(input.AppFolder);
            }
        }

        private void listCurrentLinks(LinkInput input, ApplicationManifest manifest)
        {
            var appFolder = input.AppFolder;

            ListCurrentLinks(appFolder, manifest);
        }

        public static void ListCurrentLinks(string appFolder, ApplicationManifest manifest)
        {
            if (manifest.LinkedFolders.Any())
            {
                Console.WriteLine("  Links for " + appFolder);
                manifest.LinkedFolders.Each(x => { Console.WriteLine("    " + x); });
            }
            else
            {
                Console.WriteLine("  No package links for " + appFolder);
            }
        }

        private void updateManifest(LinkInput input, FileSystem fileSystem, ApplicationManifest manifest)
        {
            if (input.RemoveFlag)
            {
                remove(input, manifest);
            }
            else
            {
                add(fileSystem, input, manifest);
            }

            persist(input, manifest, fileSystem);
        }

        private void persist(LinkInput input, ApplicationManifest manifest, FileSystem fileSystem)
        {
            fileSystem.PersistToFile(manifest, input.AppFolder, ApplicationManifest.FILE);
        }

        private void remove(LinkInput input, ApplicationManifest manifest)
        {
            manifest.RemoveLink(input.RelativePathOfPackage());
            Console.WriteLine("Folder {0} was removed from the application at {1}", input.PackageFolder, input.AppFolder);
        }

        private void add(IFileSystem system, LinkInput input, ApplicationManifest manifest)
        {
            var exists = system.FileExists(input.PackageFolder, PackageManifest.FILE);
            if (!exists)
            {
                throw new ApplicationException(
                    "There is no package manifest file for the requested package folder at " + input.PackageFolder);
            }

            var wasAdded = manifest.AddLink(input.RelativePathOfPackage());
            Console.WriteLine(
                wasAdded
                    ? "Folder {0} was added to the application at {1}"
                    : "Folder {0} is already included in the application at {1}", input.PackageFolder, input.AppFolder);
        }
    }
}