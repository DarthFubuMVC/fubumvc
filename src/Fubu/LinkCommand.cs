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
        [RequiredUsage("list", "create", "remove")]
        public string AppFolder { get; set; }

        [Description("The physical folder (or valid alias) of a package")]
        [RequiredUsage("create", "remove")]
        public string PackageFolder { get; set; }

        [Description("Remove the package folder link from the application")]
        [RequiredUsage("remove")]
        [ValidUsage("remove")]
        public bool RemoveFlag { get; set; }

        [Description("Remove all links from an application manifest file")]
        public bool CleanAllFlag { get; set; }

        [Description("Opens the package manifest file in notepad")]
        public bool NotepadFlag { get; set; }

        public string RelativePathOfPackage()
        {
            var pkg = Path.GetFullPath(PackageFolder);
            var app = Path.GetFullPath(AppFolder);

            return pkg.PathRelativeTo(app);
        }
    }

    // TODO -- do NOT delete the manifest file
    [Usage("list", "List the current links for the application")]
    [Usage("create", "Create a new link for the application to the package")]
    [Usage("remove", "Remove any existing link for the application to the package")]
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
            if (input.CleanAllFlag)
            {
                fileSystem.DeleteFile(input.AppFolder, ApplicationManifest.FILE);

                Console.WriteLine("Deleted the package include manifest file for " + input.AppFolder);
                return;
            }

            var manifest = fileSystem.LoadApplicationManifestFrom(input.AppFolder);

            if (input.PackageFolder.IsNotEmpty())
            {
                updateManifest(input, fileSystem, manifest);
            }
            else
            {
                listCurrentLinks(input, manifest);
            }


            if (input.NotepadFlag)
            {
                fileSystem.OpenInNotepad(input.AppFolder, ApplicationManifest.FILE);
            }
        }

        private void listCurrentLinks(LinkInput input, ApplicationManifest manifest)
        {
            var appFolder = input.AppFolder;

            ListCurrentLinks(appFolder, manifest);
        }

        public static void ListCurrentLinks(string appFolder, ApplicationManifest manifest)
        {
            if (manifest.Folders.Any())
            {
                Console.WriteLine("  Links for " + appFolder);
                manifest.Folders.Each(x => { Console.WriteLine("    " + x); });
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

            fileSystem.PersistToFile(manifest, input.AppFolder, ApplicationManifest.FILE);
        }

        private void remove(LinkInput input, ApplicationManifest manifest)
        {
            manifest.Exclude(input.RelativePathOfPackage());
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

            var wasAdded = manifest.Include(input.RelativePathOfPackage());
            Console.WriteLine(
                wasAdded
                    ? "Folder {0} was added to the application at {1}"
                    : "Folder {0} is already included in the application at {1}", input.PackageFolder, input.AppFolder);
        }
    }
}